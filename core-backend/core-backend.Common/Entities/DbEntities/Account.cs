using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommonCore.Entities.BaseEntity;
using core_backend.Common.Entities.Identities;

namespace core_backend.Common.Entities.DbEntities
{
    public class Account : BaseEntity<string>
    {
        [Required(ErrorMessage = "Account name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Account name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Balance cannot be negative")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [Required(ErrorMessage = "Owner ID is required")]
        [ForeignKey(nameof(ApplicationUser))]
        public string OwnerId { get; set; }

        public ApplicationUser Owner { get; set; }
    }
}