using CommonCore.Entities.BaseEntity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core_backend.Common.Entities.Identities
{
    public class ApplicationUser : IdentityUser, ICreated, IModified, IDeleted
    {
        [Column("RefreshToken")]
        public string? RefreshToken { get; set; }
        [Column("RefreshTokenValidity")]
        public DateTime? RefreshTokenValidity { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public DateTimeOffset? DateModified { get; set; }
        public DateTimeOffset? DateCreated { get; set; }
        public bool Deleted { get; set; }
        public DateTimeOffset? DateDeleted { get; set; }
    }
}
