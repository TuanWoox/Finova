using CommonCore.Entities.BaseEntity;
using core_backend.Common.Utils.Enums;

namespace core_backend.Common.Models.DTOs.CoreDTOs.Transaction
{
    public class TransactionDTO : IBaseKey<string>
    {
        public string Id { get; set; } = null!;

        public string AccountId { get; set; } = null!;

        public string? ToAccountId { get; set; }

        public decimal Amount { get; set; }

        public TransactionTypeEnum Type { get; set; }

        public string? Category { get; set; }

        public string? Note { get; set; }

        public DateTimeOffset TransactionDate { get; set; }

        public DateTimeOffset? DateCreated { get; set; }

        public DateTimeOffset? DateModified { get; set; }
    }
}
