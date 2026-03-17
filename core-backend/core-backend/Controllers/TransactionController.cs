using CommonCore.Models.DTO.HelperDTO;
using CommonCore.Pagings;
using CommonCore.Utils.Extensions;
using core_backend.Common.Interfaces.Services;
using core_backend.Common.Models.DTOs.CoreDTOs.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace core_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDTO createTransactionDTO)
        {
            ReturnResult<TransactionDTO> returnResult = new ReturnResult<TransactionDTO>();
            try
            {
                returnResult = await _transactionService.CreateTransactionAsync(createTransactionDTO);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred: {ex.Message}";
            }
            return Ok(returnResult);
        }

        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetTransactionById(string transactionId)
        {
            ReturnResult<TransactionDTO> returnResult = new ReturnResult<TransactionDTO>();
            try
            {
                returnResult = await _transactionService.GetTransactionByIdAsync(transactionId);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred: {ex.Message}";
            }
            return Ok(returnResult);
        }

        [HttpPost("my-transactions")]
        public async Task<IActionResult> GetMyTransactions([FromBody] Page<string> page, [FromQuery] DateTimeOffset? startDate, [FromQuery] DateTimeOffset? endDate)
        {
            ReturnResult<PagedData<TransactionDTO, string>> returnResult = new ReturnResult<PagedData<TransactionDTO, string>>();
            try
            {
                returnResult = await _transactionService.GetMyTransactionsAsync(page, startDate, endDate);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred: {ex.Message}";
            }
            return Ok(returnResult);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateTransaction([FromBody] UpdateTransactionDTO updateTransactionDTO)
        {
            ReturnResult<TransactionDTO> returnResult = new ReturnResult<TransactionDTO>();
            try
            {
                returnResult = await _transactionService.UpdateTransactionAsync(updateTransactionDTO);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred: {ex.Message}";
            }
            return Ok(returnResult);
        }

        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> DeleteTransaction(string transactionId)
        {
            ReturnResult<bool> returnResult = new ReturnResult<bool>();
            try
            {
                returnResult = await _transactionService.DeleteTransactionAsync(transactionId);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred: {ex.Message}";
            }
            return Ok(returnResult);
        }

        [HttpPost("deletemany")]
        public async Task<IActionResult> DeleteTransactions([FromBody] Page<string> page)
        {
            ReturnResult<int> returnResult = new ReturnResult<int>();
            try
            {
                returnResult = await _transactionService.DeleteTransactionsAsync(page.Selected);
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
