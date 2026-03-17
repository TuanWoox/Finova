using AutoMapper;
using CommonCore.Models.DTO.HelperDTO;
using CommonCore.Pagings;
using CommonCore.Repository;
using CommonCore.Utils.Extensions;
using core_backend.Common.Entities.DbEntities;
using core_backend.Common.Interfaces.Contexts;
using core_backend.Common.Interfaces.Services;
using core_backend.Common.Models.DTOs.CoreDTOs.Account;
using core_backend.Data;
using Microsoft.EntityFrameworkCore;

namespace core_backend.Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<Account, string> _accountRepository;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public AccountService(
            IRepository<Account, string> accountRepository,
            IUserContext userContext,
            IMapper mapper,
            ApplicationDbContext dbContext
        )
        {
            _accountRepository = accountRepository;
            _userContext = userContext;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<ReturnResult<AccountDTO>> CreateAccountAsync(CreateAccountDTO createAccountDTO)
        {
            ReturnResult<AccountDTO> returnResult = new ReturnResult<AccountDTO>();
            try
            {
                // Step 1: Validate user is authenticated
                if (string.IsNullOrEmpty(_userContext.UserId))
                {
                    returnResult.Message = "User not authenticated";
                    return returnResult;
                }

                // Step 2: Validate input
                if (string.IsNullOrWhiteSpace(createAccountDTO.Name))
                {
                    returnResult.Message = "Invalid account name";
                    return returnResult;
                }

                // Step 3: Check for duplicate account name
                var existingAccount = await _dbContext.Accounts
                    .FirstOrDefaultAsync(a => a.Name == createAccountDTO.Name && a.OwnerId == _userContext.UserId);
                if (existingAccount != null)
                {
                    returnResult.Message = $"Account with name '{createAccountDTO.Name}' already exists";
                    return returnResult;
                }

                // Step 4: Set owner to current user
                createAccountDTO.OwnerId = _userContext.UserId;

                // Step 5: Create account via repository
                var result = await _accountRepository.CreateAsync<CreateAccountDTO>(createAccountDTO);

                if (result.Result != null)
                {
                    var accountDTO = _mapper.Map<AccountDTO>(result.Result);
                    accountDTO.OwnerName = _userContext.UserName;
                    returnResult.Result = accountDTO;
                }
                else
                {
                    returnResult.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred while creating account: {ex.Message}";
            }
            return returnResult;
        }
        public async Task<ReturnResult<AccountDTO>> GetAccountByIdAsync(string accountId)
        {
            ReturnResult<AccountDTO> returnResult = new ReturnResult<AccountDTO>();
            try
            {
                // Step 1: Validate inputs
                if (string.IsNullOrEmpty(accountId))
                {
                    returnResult.Message = "Invalid account ID";
                    return returnResult;
                }

                if (string.IsNullOrEmpty(_userContext.UserId))
                {
                    returnResult.Message = "User not authenticated";
                    return returnResult;
                }

                // Step 2: Get account
                var result = await _accountRepository.GetByIdAsync(accountId);

                if (result.Result == null)
                {
                    returnResult.Message = $"Account {accountId} not found";
                    return returnResult;
                }

                // Step 3: Verify ownership
                if (result.Result.OwnerId != _userContext.UserId)
                {
                    returnResult.Message = "Access denied: You do not own this account";
                    return returnResult;
                }

                // Step 4: Map and return
                var accountDTO = _mapper.Map<AccountDTO>(result.Result);
                accountDTO.OwnerName = _userContext.UserName;
                returnResult.Result = accountDTO;
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred while retrieving account: {ex.Message}";
            }
            return returnResult;
        }
        public async Task<ReturnResult<PagedData<AccountDTO, string>>> GetMyAccountsAsync(Page<string> page)
        {
            ReturnResult<PagedData<AccountDTO, string>> returnResult = new ReturnResult<PagedData<AccountDTO, string>>();
            try
            {
                // Step 1: Validate user is authenticated
                if (string.IsNullOrEmpty(_userContext.UserId))
                {
                    returnResult.Message = "User not authenticated";
                    return returnResult;
                }

                // Step 2: Validate page object
                if (page == null)
                {
                    returnResult.Message = "Invalid page parameters";
                    return returnResult;
                }

                // Step 3: Create filtered query for current user's accounts
                var query = _dbContext.Accounts.Where(a => a.OwnerId == _userContext.UserId).AsQueryable();

                // Step 4: Get paginated accounts from repository
                var pagedData = await _accountRepository.GetPagingAsync<Page<string>, AccountDTO>(query, page);

                // Step 5: Map owner name to all results
                if (pagedData.Data != null)
                {
                    foreach (var accountDTO in pagedData.Data)
                    {
                        accountDTO.OwnerName = _userContext.UserName;
                    }
                }

                returnResult.Result = pagedData;
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred while retrieving accounts: {ex.Message}";
            }
            return returnResult;
        }
        public async Task<ReturnResult<AccountDTO>> UpdateAccountAsync(UpdateAccountDTO updateAccountDTO)
        {
            ReturnResult<AccountDTO> returnResult = new ReturnResult<AccountDTO>();
            try
            {
                // Step 1: Validate inputs
                if (string.IsNullOrEmpty(updateAccountDTO.Id))
                {
                    returnResult.Message = "Invalid account ID";
                    return returnResult;
                }

                if (string.IsNullOrEmpty(_userContext.UserId))
                {
                    returnResult.Message = "User not authenticated";
                    return returnResult;
                }

                // Step 2: Get existing account
                var getResult = await _accountRepository.GetByIdAsync(updateAccountDTO.Id);

                if (getResult.Result == null)
                {
                    returnResult.Message = $"Account {updateAccountDTO.Id} not found";
                    return returnResult;
                }

                // Step 3: Verify ownership
                if (getResult.Result.OwnerId != _userContext.UserId)
                {
                    returnResult.Message = "Access denied: You do not own this account";
                    return returnResult;
                }

                // Step 4: Check for duplicate account name (excluding current account)
                var existingAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Name == updateAccountDTO.Name &&
                                                                a.OwnerId == _userContext.UserId &&
                                                                a.Id != updateAccountDTO.Id);
                if (existingAccount != null)
                {
                    returnResult.Message = $"Account with name '{updateAccountDTO.Name}' already exists";
                    return returnResult;
                }

                // Step 5: Update account
                var result = await _accountRepository.UpdateAsync<UpdateAccountDTO>(updateAccountDTO);

                if (result.Result != null)
                {
                    var accountDTO = _mapper.Map<AccountDTO>(result.Result);
                    accountDTO.OwnerName = _userContext.UserName;
                    returnResult.Result = accountDTO;
                }
                else
                {
                    returnResult.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred while updating account: {ex.Message}";
            }
            return returnResult;
        }
        public async Task<ReturnResult<bool>> DeleteAccountAsync(string accountId)
        {
            ReturnResult<bool> returnResult = new ReturnResult<bool>();
            try
            {
                // Step 1: Validate inputs
                if (string.IsNullOrEmpty(accountId))
                {
                    returnResult.Message = "Invalid account ID";
                    return returnResult;
                }

                if (string.IsNullOrEmpty(_userContext.UserId))
                {
                    returnResult.Message = "User not authenticated";
                    return returnResult;
                }

                // Step 2: Get existing account
                var getResult = await _accountRepository.GetByIdAsync(accountId);

                if (getResult.Result == null)
                {
                    returnResult.Message = $"Account {accountId} not found";
                    return returnResult;
                }

                // Step 3: Verify ownership
                if (getResult.Result.OwnerId != _userContext.UserId)
                {
                    returnResult.Message = "Access denied: You do not own this account";
                    return returnResult;
                }

                // Step 4: Delete account
                var result = await _accountRepository.DeleteByIdAsync(accountId);
                returnResult.Result = result.Result;

                if (!result.Result)
                {
                    returnResult.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred while deleting account: {ex.Message}";
            }
            return returnResult;
        }

        public async Task<ReturnResult<int>> DeletAccountsAsync(List<string> ids)
        {
            ReturnResult<int> returnResult = new ReturnResult<int>();
            try
            {
                // Step 1: Validate inputs
                if (ids == null || ids.Count == 0)
                {
                    returnResult.Message = "Invalid account IDs";
                    return returnResult;
                }

                // Step 2: Check authentication
                if (string.IsNullOrEmpty(_userContext.UserId))
                {
                    returnResult.Message = "User not authenticated";
                    return returnResult;
                }

                // Step 3: Verify all requested accounts are owned by current user
                var ownedAccountsCount = await _dbContext.Accounts
                    .Where(a => ids.Contains(a.Id) && a.OwnerId == _userContext.UserId)
                    .CountAsync();

                if (ownedAccountsCount != ids.Count)
                {
                    returnResult.Message = "Access denied: You do not own all requested accounts";
                    return returnResult;
                }

                // Step 4: Delete all accounts via repository
                var result = await _accountRepository.DeleteByIdsAsync(ids);
                returnResult.Result = result.Result;

                if (result.Result == 0)
                {
                    returnResult.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred while deleting accounts: {ex.Message}";
            }
            return returnResult;
        }
    }
}
