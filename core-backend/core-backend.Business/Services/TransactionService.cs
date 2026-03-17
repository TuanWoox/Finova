using AutoMapper;
using CommonCore.Models.DTO.HelperDTO;
using CommonCore.Pagings;
using CommonCore.Repository;
using CommonCore.Utils.Extensions;
using core_backend.Common.Entities.DbEntities;
using core_backend.Common.Interfaces.Contexts;
using core_backend.Common.Interfaces.Services;
using core_backend.Common.Models.DTOs.CoreDTOs.Transaction;
using core_backend.Common.Utils.Enums;
using core_backend.Data;
using Microsoft.EntityFrameworkCore;

namespace core_backend.Business.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<Transaction, string> _transactionRepository;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public TransactionService(
            IRepository<Transaction, string> transactionRepository,
            IUserContext userContext,
            IMapper mapper,
            ApplicationDbContext dbContext
        )
        {
            _transactionRepository = transactionRepository;
            _userContext = userContext;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<ReturnResult<TransactionDTO>> CreateTransactionAsync(CreateTransactionDTO createTransactionDTO)
        {
            ReturnResult<TransactionDTO> returnResult = new ReturnResult<TransactionDTO>();
            try
            {
                if (string.IsNullOrEmpty(_userContext.UserId))
                {
                    returnResult.Message = "User not authenticated";
                    return returnResult;
                }

                if (string.IsNullOrWhiteSpace(createTransactionDTO.AccountId))
                {
                    returnResult.Message = "Invalid account ID";
                    return returnResult;
                }

                if (createTransactionDTO.Amount <= 0)
                {
                    returnResult.Message = "Amount must be greater than 0";
                    return returnResult;
                }

                var ownsSourceAccount = await _dbContext.Accounts
                    .AnyAsync(a => a.Id == createTransactionDTO.AccountId && a.OwnerId == _userContext.UserId);

                if (!ownsSourceAccount)
                {
                    returnResult.Message = "Access denied: You do not own this account";
                    return returnResult;
                }

                var transferValidationMessage = await ValidateTransferRulesAsync(
                    createTransactionDTO.Type,
                    createTransactionDTO.AccountId,
                    createTransactionDTO.ToAccountId
                );
                if (!string.IsNullOrEmpty(transferValidationMessage))
                {
                    returnResult.Message = transferValidationMessage;
                    return returnResult;
                }

                var result = await _transactionRepository.CreateAsync<CreateTransactionDTO>(createTransactionDTO);
                if (result.Result != null)
                {
                    returnResult.Result = _mapper.Map<TransactionDTO>(result.Result);
                }
                else
                {
                    returnResult.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred while creating transaction: {ex.Message}";
            }
            return returnResult;
        }

        public async Task<ReturnResult<TransactionDTO>> GetTransactionByIdAsync(string transactionId)
        {
            ReturnResult<TransactionDTO> returnResult = new ReturnResult<TransactionDTO>();
            try
            {
                if (string.IsNullOrWhiteSpace(transactionId))
                {
                    returnResult.Message = "Invalid transaction ID";
                    return returnResult;
                }

                if (string.IsNullOrEmpty(_userContext.UserId))
                {
                    returnResult.Message = "User not authenticated";
                    return returnResult;
                }

                var transaction = await _dbContext.Transactions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == transactionId && t.Account.OwnerId == _userContext.UserId);

                if (transaction == null)
                {
                    returnResult.Message = $"Transaction {transactionId} not found";
                    return returnResult;
                }

                returnResult.Result = _mapper.Map<TransactionDTO>(transaction);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred while retrieving transaction: {ex.Message}";
            }
            return returnResult;
        }

        public async Task<ReturnResult<PagedData<TransactionDTO, string>>> GetMyTransactionsAsync(Page<string> page, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            ReturnResult<PagedData<TransactionDTO, string>> returnResult = new ReturnResult<PagedData<TransactionDTO, string>>();
            try
            {
                if (string.IsNullOrEmpty(_userContext.UserId))
                {
                    returnResult.Message = "User not authenticated";
                    return returnResult;
                }

                if (page == null)
                {
                    returnResult.Message = "Invalid page parameters";
                    return returnResult;
                }

                if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                {
                    returnResult.Message = "Invalid date range";
                    return returnResult;
                }

                var query = _dbContext.Transactions
                    .Where(t => t.Account.OwnerId == _userContext.UserId)
                    .AsQueryable();

                if (startDate.HasValue)
                {
                    query = query.Where(t => t.TransactionDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(t => t.TransactionDate <= endDate.Value);
                }

                var pagedData = await _transactionRepository.GetPagingAsync<Page<string>, TransactionDTO>(query, page);
                returnResult.Result = pagedData;
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred while retrieving transactions: {ex.Message}";
            }
            return returnResult;
        }

        public async Task<ReturnResult<TransactionDTO>> UpdateTransactionAsync(UpdateTransactionDTO updateTransactionDTO)
        {
            ReturnResult<TransactionDTO> returnResult = new ReturnResult<TransactionDTO>();
            try
            {
                if (string.IsNullOrWhiteSpace(updateTransactionDTO.Id))
                {
                    returnResult.Message = "Invalid transaction ID";
                    return returnResult;
                }

                if (string.IsNullOrEmpty(_userContext.UserId))
                {
                    returnResult.Message = "User not authenticated";
                    return returnResult;
                }

                if (string.IsNullOrWhiteSpace(updateTransactionDTO.AccountId))
                {
                    returnResult.Message = "Invalid account ID";
                    return returnResult;
                }

                if (updateTransactionDTO.Amount <= 0)
                {
                    returnResult.Message = "Amount must be greater than 0";
                    return returnResult;
                }

                var currentTransaction = await _dbContext.Transactions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == updateTransactionDTO.Id);

                if (currentTransaction == null)
                {
                    returnResult.Message = $"Transaction {updateTransactionDTO.Id} not found";
                    return returnResult;
                }

                var ownsCurrentTransaction = await _dbContext.Accounts
                    .AnyAsync(a => a.Id == currentTransaction.AccountId && a.OwnerId == _userContext.UserId);

                if (!ownsCurrentTransaction)
                {
                    returnResult.Message = "Access denied: You do not own this transaction";
                    return returnResult;
                }

                var ownsTargetAccount = await _dbContext.Accounts
                    .AnyAsync(a => a.Id == updateTransactionDTO.AccountId && a.OwnerId == _userContext.UserId);

                if (!ownsTargetAccount)
                {
                    returnResult.Message = "Access denied: You do not own this account";
                    return returnResult;
                }

                var transferValidationMessage = await ValidateTransferRulesAsync(
                    updateTransactionDTO.Type,
                    updateTransactionDTO.AccountId,
                    updateTransactionDTO.ToAccountId
                );
                if (!string.IsNullOrEmpty(transferValidationMessage))
                {
                    returnResult.Message = transferValidationMessage;
                    return returnResult;
                }

                var result = await _transactionRepository.UpdateAsync<UpdateTransactionDTO>(updateTransactionDTO);

                if (result.Result != null)
                {
                    returnResult.Result = _mapper.Map<TransactionDTO>(result.Result);
                }
                else
                {
                    returnResult.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred while updating transaction: {ex.Message}";
            }
            return returnResult;
        }

        public async Task<ReturnResult<bool>> DeleteTransactionAsync(string transactionId)
        {
            ReturnResult<bool> returnResult = new ReturnResult<bool>();
            try
            {
                if (string.IsNullOrWhiteSpace(transactionId))
                {
                    returnResult.Message = "Invalid transaction ID";
                    return returnResult;
                }

                if (string.IsNullOrEmpty(_userContext.UserId))
                {
                    returnResult.Message = "User not authenticated";
                    return returnResult;
                }

                var transaction = await _dbContext.Transactions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == transactionId);

                if (transaction == null)
                {
                    returnResult.Message = $"Transaction {transactionId} not found";
                    return returnResult;
                }

                var ownsTransaction = await _dbContext.Accounts
                    .AnyAsync(a => a.Id == transaction.AccountId && a.OwnerId == _userContext.UserId);

                if (!ownsTransaction)
                {
                    returnResult.Message = "Access denied: You do not own this transaction";
                    return returnResult;
                }

                var result = await _transactionRepository.DeleteByIdAsync(transactionId);
                returnResult.Result = result.Result;

                if (!result.Result)
                {
                    returnResult.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred while deleting transaction: {ex.Message}";
            }
            return returnResult;
        }

        public async Task<ReturnResult<int>> DeleteTransactionsAsync(List<string> ids)
        {
            ReturnResult<int> returnResult = new ReturnResult<int>();
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    returnResult.Message = "Invalid transaction IDs";
                    return returnResult;
                }

                if (string.IsNullOrEmpty(_userContext.UserId))
                {
                    returnResult.Message = "User not authenticated";
                    return returnResult;
                }

                var ownedTransactionsCount = await _dbContext.Transactions
                    .Where(t => ids.Contains(t.Id) && t.Account.OwnerId == _userContext.UserId)
                    .CountAsync();

                if (ownedTransactionsCount != ids.Count)
                {
                    returnResult.Message = "Access denied: You do not own all requested transactions";
                    return returnResult;
                }

                var result = await _transactionRepository.DeleteByIdsAsync(ids);
                returnResult.Result = result.Result;

                if (result.Result == 0)
                {
                    returnResult.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Debug(ex.Message);
                returnResult.Message = $"An error occurred while deleting transactions: {ex.Message}";
            }
            return returnResult;
        }

        private async Task<string?> ValidateTransferRulesAsync(TransactionTypeEnum type, string accountId, string? toAccountId)
        {
            if (type == TransactionTypeEnum.Transfer)
            {
                if (string.IsNullOrWhiteSpace(toAccountId))
                {
                    return "To account is required for transfer transactions";
                }

                if (accountId == toAccountId)
                {
                    return "Transfer source and destination accounts must be different";
                }

                var ownsDestinationAccount = await _dbContext.Accounts
                    .AnyAsync(a => a.Id == toAccountId && a.OwnerId == _userContext.UserId);

                if (!ownsDestinationAccount)
                {
                    return "Access denied: You do not own destination account";
                }
            }
            else if (!string.IsNullOrWhiteSpace(toAccountId))
            {
                return "To account is only allowed for transfer transactions";
            }

            return null;
        }
    }
}
