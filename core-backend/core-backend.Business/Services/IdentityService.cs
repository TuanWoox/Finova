using CommonCore.Utils.Extensions;
using core_backend.Common.Entities.Identities;
using core_backend.Common.Interfaces.Services;
using core_backend.Common.Utils.Enums;
using core_backend.Data;
using Microsoft.AspNetCore.Identity;

namespace core_backend.Business.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public IdentityService(ApplicationDbContext dbContext, RoleManager<ApplicationRole> roleManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
        }
        public async Task<bool> InitRole()
        {
            bool initResult = false;
            try
            {
                var roleNames = Enum.GetNames(typeof(RoleEnum)).ToList();
                foreach (var roleName in roleNames)
                {
                    var roleExist = await _roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        var newRole = new ApplicationRole
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = roleName,
                            NormalizedName = roleName.ToUpper(),
                            DateCreated = DateTimeOffset.UtcNow,
                            Deleted = false,
                        };
                        var result = await _roleManager.CreateAsync(newRole);
                        if (!result.Succeeded)
                        {
                            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                            AppLogger.Instance.Error($"Failed to create role '{roleName}': {errors}");
                            throw new InvalidOperationException($"Failed to create role '{roleName}': {errors}");
                        }
                        else initResult = true;
                    }
                }
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Error($"An error occurred while initializing roles: {ex.Message}", ex);
                throw;
            }
            return initResult;
        }
    }
}