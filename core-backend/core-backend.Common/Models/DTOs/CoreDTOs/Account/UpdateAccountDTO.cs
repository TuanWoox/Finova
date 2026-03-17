using System.ComponentModel.DataAnnotations;
using CommonCore.Entities.BaseEntity;

namespace core_backend.Common.Models.DTOs.CoreDTOs.Account
{
    public class UpdateAccountDTO : IBaseKey<string>
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal Balance { get; set; }
    }
}
