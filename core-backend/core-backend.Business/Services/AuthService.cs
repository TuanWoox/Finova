using CommonCore.Models.DTO.HelperDTO;
using CommonCore.Utils.Extensions;
using core_backend.Common.Entities.Identities;
using core_backend.Common.Interfaces.Services;
using core_backend.Common.Models.DTOs.CoreDTOs.Auth;
using core_backend.Common.Models.DTOs.HelperDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace core_backend.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<ReturnResult<bool>> RegisterAccount(RegisterAccountDTO registerAccountDTO)
        {
            ReturnResult<bool> returnResult = new ReturnResult<bool>();
            try
            {
                // Check if email already exists
                var existingUser = await _userManager.FindByEmailAsync(registerAccountDTO.Email);
                if (existingUser != null)
                {
                    returnResult.Message = $"Email {registerAccountDTO.Email} already registered";
                    return returnResult;
                }

                // Check if username already exists
                var existingUserName = await _userManager.FindByNameAsync(registerAccountDTO.UserName);
                if (existingUserName != null)
                {
                    returnResult.Message = $"Username ${registerAccountDTO.UserName} already taken";
                    return returnResult;
                }

                // Create new user
                var user = new ApplicationUser
                {
                    Email = registerAccountDTO.Email,
                    UserName = registerAccountDTO.UserName,
                    FirstName = registerAccountDTO.FirstName,
                    LastName = registerAccountDTO.LastName,
                    DateOfBirth = registerAccountDTO.DateOfBirth,
                    DateCreated = DateTimeOffset.UtcNow
                };

                var result = await _userManager.CreateAsync(user, registerAccountDTO.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    returnResult.Message = $"Registration failed: {errors}";
                    return returnResult;
                }

                // Assign default "User" role
                var roleAssignResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleAssignResult.Succeeded)
                {
                    var errors = string.Join(", ", roleAssignResult.Errors.Select(e => e.Description));
                    returnResult.Message = $"User created but role assignment failed: {errors}";
                    return returnResult;
                }

                returnResult.Result = true;
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred during registration: {ex.Message}";
            }
            return returnResult;
        }
        public async Task<ReturnResult<TokenResponse>> LoginAccount(LoginAccountDTO loginAccountDTO)
        {
            ReturnResult<TokenResponse> returnResult = new ReturnResult<TokenResponse>();
            try
            {
                // Find user by email or username
                var user = await _userManager.FindByEmailAsync(loginAccountDTO.UserName)
                    ?? await _userManager.FindByNameAsync(loginAccountDTO.UserName);

                if (user == null)
                {
                    returnResult.Message = "Invalid email/username or password";
                    return returnResult;
                }

                // Verify password
                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginAccountDTO.Password, false);
                if (!signInResult.Succeeded)
                {
                    returnResult.Message = "Invalid email/username or password";
                    return returnResult;
                }

                // Generate tokens
                var accessToken = GenerateAccessToken(user);
                var refreshToken = GenerateRefreshToken();

                // Set refresh token validity based on RememberMe
                var refreshTokenValidity = loginAccountDTO.RememberMe
                    ? DateTime.UtcNow.AddDays(30)  // 30 days for RememberMe
                    : DateTime.UtcNow.AddDays(7);  // 7 days for normal login

                // Update refresh token in database
                user.RefreshToken = refreshToken;
                user.RefreshTokenValidity = refreshTokenValidity;
                user.DateModified = DateTimeOffset.UtcNow;

                await _userManager.UpdateAsync(user);

                returnResult.Result = new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred during login: {ex.Message}";
            }
            return returnResult;
        }
        private string GenerateAccessToken(ApplicationUser user)
        {
            // TODO: Move these to configuration (appsettings.json)
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm"));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            // Add user roles to claims
            var userRoles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), // 1 hour validity
                Issuer = "YourAppName",
                Audience = "YourAppUsers",
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
