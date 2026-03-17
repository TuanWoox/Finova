using AutoMapper;
using core_backend.Common.Entities.DbEntities;
using core_backend.Common.Entities.Identities;
using core_backend.Common.Models.DTOs.CoreDTOs.Account;
using core_backend.Common.Models.DTOs.CoreDTOs.Auth;

namespace core_backend.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Auth Mappings
            CreateMap<RegisterAccountDTO, ApplicationUser>();

            // Account Mappings
            CreateMap<CreateAccountDTO, Account>();
            CreateMap<UpdateAccountDTO, Account>();
            CreateMap<Account, AccountDTO>()
                .ForMember(dest => dest.OwnerName, opt => opt.Ignore());
        }
    }
}
