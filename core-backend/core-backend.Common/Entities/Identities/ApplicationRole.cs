using CommonCore.Entities.BaseEntity;
using Microsoft.AspNetCore.Identity;


namespace core_backend.Common.Entities.Identities
{
    public class ApplicationRole : IdentityRole<string>, ICreated, IDeleted, IModified
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual DateTimeOffset? DateCreated { get; set; }

        public virtual DateTimeOffset? DateModified { get; set; }

        public virtual bool Deleted { get; set; }
        public virtual DateTimeOffset? DateDeleted { get; set; }
    }
}
