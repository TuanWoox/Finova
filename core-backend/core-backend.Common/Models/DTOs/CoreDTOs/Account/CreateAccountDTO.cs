using System.ComponentModel.DataAnnotations;
using CommonCore.Entities.BaseEntity;

namespace core_backend.Common.Models.DTOs.CoreDTOs.Account
{
    public class CreateAccountDTO : IBaseKey<string>
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal InitialBalance { get; set; } = 0;

        public string? OwnerId { get; set; }
    }
}
