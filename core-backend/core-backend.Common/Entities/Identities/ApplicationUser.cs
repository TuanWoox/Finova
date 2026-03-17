using CommonCore.Entities.BaseEntity;
using core_backend.Common.Entities.DbEntities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core_backend.Common.Entities.Identities
{
    public class ApplicationUser : IdentityUser, ICreated, IModified, IDeleted
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 100 characters")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 100 characters")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenValidity { get; set; }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

        public ICollection<Account> Accounts = new List<Account>();

        public DateTimeOffset? DateModified { get; set; }

        public DateTimeOffset? DateCreated { get; set; }

        public bool Deleted { get; set; }

        public DateTimeOffset? DateDeleted { get; set; }
    }
}
