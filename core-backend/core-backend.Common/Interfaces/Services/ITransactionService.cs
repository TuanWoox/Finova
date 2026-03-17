using CommonCore.Models.DTO.HelperDTO;
using CommonCore.Pagings;
using core_backend.Common.Models.DTOs.CoreDTOs.Transaction;

namespace core_backend.Common.Interfaces.Services
{
    public interface ITransactionService
    {
        public Task<ReturnResult<TransactionDTO>> CreateTransactionAsync(CreateTransactionDTO createTransactionDTO);
        public Task<ReturnResult<TransactionDTO>> GetTransactionByIdAsync(string transactionId);
        public Task<ReturnResult<PagedData<TransactionDTO, string>>> GetMyTransactionsAsync(Page<string> page, DateTimeOffset? startDate, DateTimeOffset? endDate);
        public Task<ReturnResult<TransactionDTO>> UpdateTransactionAsync(UpdateTransactionDTO updateTransactionDTO);
        public Task<ReturnResult<bool>> DeleteTransactionAsync(string transactionId);
        public Task<ReturnResult<int>> DeleteTransactionsAsync(List<string> ids);
    }
}
