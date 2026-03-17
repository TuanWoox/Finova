using CommonCore.Models.DTO.HelperDTO;
using core_backend.Common.Models.DTOs.CoreDTOs.Auth;
using core_backend.Common.Models.DTOs.HelperDTOs;

namespace core_backend.Common.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<ReturnResult<bool>> RegisterAccount(RegisterAccountDTO registerAccountDTO);
        public Task<ReturnResult<TokenResponse>> LoginAccount(LoginAccountDTO loginAccountDTO);
    }
}