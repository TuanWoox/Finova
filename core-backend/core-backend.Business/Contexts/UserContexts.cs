using System.Security.Claims;
using CommonCore.Utils.Extensions;
using core_backend.Common.Interfaces.Contexts;
using core_backend.Common.Utils.Enums;
using Microsoft.AspNetCore.Http;

namespace core_backend.Business.Contexts
{
    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string UserId => _httpContextAccessor.HttpContext?.User?.GetUserId() ?? "";
        public bool IsAdmin => _httpContextAccessor.HttpContext?.User?.IsInRole(RoleEnum.Admin.ToString()) ?? false;
        public string UserName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "";

        public string Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? "";
    }
}