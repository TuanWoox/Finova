namespace core_backend.Common.Interfaces.Contexts
{
    public interface IUserContext
    {
        string UserId { get; }
        bool IsAdmin { get; }
        string UserName { get; }
        string Email { get; }
    }
}