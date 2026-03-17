using CommonCore.Models.DTO.HelperDTO;
using CommonCore.Pagings;
using core_backend.Common.Models.DTOs.CoreDTOs.Account;

namespace core_backend.Common.Interfaces.Services
{
    public interface IAccountService
    {
        public Task<ReturnResult<AccountDTO>> CreateAccountAsync(CreateAccountDTO createAccountDTO);
        public Task<ReturnResult<AccountDTO>> GetAccountByIdAsync(string accountId);
        public Task<ReturnResult<PagedData<AccountDTO, string>>> GetMyAccountsAsync(Page<string> page);
        public Task<ReturnResult<AccountDTO>> UpdateAccountAsync(UpdateAccountDTO updateAccountDTO);
        public Task<ReturnResult<bool>> DeleteAccountAsync(string accountId);
        public Task<ReturnResult<int>> DeletAccountsAsync(List<string> ids);
    }
}
