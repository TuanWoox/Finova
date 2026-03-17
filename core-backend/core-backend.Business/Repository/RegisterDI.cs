using System.Security.Principal;
using CommonCore.Repository;
using core_backend.Business.Contexts;
using core_backend.Business.Services;
using core_backend.Common.Interfaces.Contexts;
using core_backend.Common.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace core_backend.Business.Repository
{
    public static class RegisterDI
    {
        public static IServiceCollection RegisterServiceDI(this IServiceCollection services)
        {
            services.TryAddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.TryAddScoped<IUserContext, UserContext>();
            services.TryAddScoped<IIdentityService, IdentityService>();
            services.TryAddScoped<IAuthService, AuthService>();
            services.TryAddScoped<IAccountService, AccountService>();
            services.TryAddScoped<ITransactionService, TransactionService>();
            return services;
        }
    }
}