using System.ComponentModel.DataAnnotations;

namespace core_backend.Common.Models.DTOs.CoreDTOs.Auth
{
    public class LoginAccountDTO
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; } = false;
    }
}
