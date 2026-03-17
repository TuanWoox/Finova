using AutoMapper;
using core_backend.Common.Entities.DbEntities;
using core_backend.Common.Entities.Identities;
using core_backend.Common.Models.DTOs.CoreDTOs.Account;
using core_backend.Common.Models.DTOs.CoreDTOs.Auth;
using core_backend.Common.Models.DTOs.CoreDTOs.Transaction;

namespace core_backend.Business.Mappings
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

            // Transaction Mappings
            CreateMap<CreateTransactionDTO, Transaction>();
            CreateMap<UpdateTransactionDTO, Transaction>();
            CreateMap<Transaction, TransactionDTO>();
        }
    }
}
