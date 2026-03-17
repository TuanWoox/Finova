using CommonCore.Entities.BaseEntity;

namespace core_backend.Common.Models.DTOs.CoreDTOs.Account
{
    public class AccountDTO : IBaseKey<string>
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public decimal Balance { get; set; }

        public string OwnerId { get; set; } = null!;

        public string OwnerName { get; set; } = null!;

        public DateTimeOffset? DateCreated { get; set; }

        public DateTimeOffset? DateModified { get; set; }
    }
}
