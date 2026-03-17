using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommonCore.Entities.BaseEntity;
using core_backend.Common.Utils.Enums;

namespace core_backend.Common.Entities.DbEntities
{
    public class Transaction : BaseEntity<string>
    {
        [Required(ErrorMessage = "Account ID is required")]
        [ForeignKey(nameof(Account))]
        public string AccountId { get; set; } = null!;

        public Account Account { get; set; } = null!;

        [ForeignKey(nameof(ToAccount))]
        public string? ToAccountId { get; set; }

        public Account? ToAccount { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Transaction type is required")]
        public TransactionTypeEnum Type { get; set; }

        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string? Category { get; set; }

        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        public string? Note { get; set; }

        public DateTimeOffset TransactionDate { get; set; } = DateTimeOffset.UtcNow;
    }
}
