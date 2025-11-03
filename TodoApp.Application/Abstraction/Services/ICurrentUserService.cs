namespace TodoApp.Application.Abstraction.Services
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? DomainUserId { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
    }
}