using CommonCore.Models.DTO.HelperDTO;
using CommonCore.Pagings;
using CommonCore.Utils.Extensions;
using core_backend.Common.Interfaces.Services;
using core_backend.Common.Models.DTOs.CoreDTOs.Account;
using core_backend.Common.Models.DTOs.HelperDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace core_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Create a new account for the current user
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDTO createAccountDTO)
        {
            ReturnResult<AccountDTO> returnResult = new ReturnResult<AccountDTO>();
            try
            {
                returnResult = await _accountService.CreateAccountAsync(createAccountDTO);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred: {ex.Message}";
            }
            return Ok(returnResult);
        }

        /// <summary>
        /// Get account by ID (must be owner)
        /// </summary>
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccountById(string accountId)
        {
            ReturnResult<AccountDTO> returnResult = new ReturnResult<AccountDTO>();
            try
            {
                returnResult = await _accountService.GetAccountByIdAsync(accountId);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred: {ex.Message}";
            }
            return Ok(returnResult);
        }

        /// <summary>
        /// Get all accounts for the current user with pagination
        /// </summary>
        [HttpPost("my-accounts")]
        public async Task<IActionResult> GetMyAccounts([FromBody] Page<string> page)
        {
            ReturnResult<PagedData<AccountDTO, string>> returnResult = new ReturnResult<PagedData<AccountDTO, string>>();
            try
            {
                returnResult = await _accountService.GetMyAccountsAsync(page);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred: {ex.Message}";
            }
            return Ok(returnResult);
        }

        /// <summary>
        /// Update an account (must be owner)
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountDTO updateAccountDTO)
        {
            ReturnResult<AccountDTO> returnResult = new ReturnResult<AccountDTO>();
            try
            {
                returnResult = await _accountService.UpdateAccountAsync(updateAccountDTO);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred: {ex.Message}";
            }
            return Ok(returnResult);
        }

        /// <summary>
        /// Delete an account (must be owner)
        /// </summary>
        [HttpDelete("{accountId}")]
        public async Task<IActionResult> DeleteAccount(string accountId)
        {
            ReturnResult<bool> returnResult = new ReturnResult<bool>();
            try
            {
                returnResult = await _accountService.DeleteAccountAsync(accountId);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred: {ex.Message}";
            }
            return Ok(returnResult);
        }

        [HttpPost("deletemany")]
        public async Task<IActionResult> DeleteAccounts(Page<string> page)
        {
            ReturnResult<int> returnResult = new ReturnResult<int>();
            try
            {
                returnResult = await _accountService.DeletAccountsAsync(page.Selected);
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

