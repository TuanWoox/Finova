using System.ComponentModel.DataAnnotations;
using CommonCore.Entities.BaseEntity;
using core_backend.Common.Utils.Enums;

namespace core_backend.Common.Models.DTOs.CoreDTOs.Transaction
{
    public class CreateTransactionDTO : IBaseKey<string>
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string AccountId { get; set; } = null!;

        public string? ToAccountId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public TransactionTypeEnum Type { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        public DateTimeOffset TransactionDate { get; set; } = DateTimeOffset.UtcNow;
    }
}
