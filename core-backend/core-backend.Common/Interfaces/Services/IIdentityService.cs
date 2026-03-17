namespace core_backend.Common.Interfaces.Services
{
    public interface IIdentityService
    {
        public Task<bool> InitRole();
    }
}