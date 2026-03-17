using CommonCore.Models.DTO.HelperDTO;
using CommonCore.Utils.Extensions;
using core_backend.Common.Interfaces.Services;
using core_backend.Common.Models.DTOs.CoreDTOs.Auth;
using core_backend.Common.Models.DTOs.HelperDTOs;
using Microsoft.AspNetCore.Mvc;

namespace core_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private IAuthService _authSerive;

        public AuthController(IAuthService authService)
        {
            _authSerive = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAccount(RegisterAccountDTO registerAccountDTO)
        {
            ReturnResult<bool> returnResult = new ReturnResult<bool>();
            try
            {
                returnResult = await _authSerive.RegisterAccount(registerAccountDTO);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred: {ex.Message}";
            }
            return Ok(returnResult);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAccount(LoginAccountDTO loginAccountDTO)
        {
            ReturnResult<TokenResponse> returnResult = new ReturnResult<TokenResponse>();
            try
            {
                returnResult = await _authSerive.LoginAccount(loginAccountDTO);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred: {ex.Message}";
            }
            return Ok(returnResult);
        }
    }
}