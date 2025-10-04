namespace MetaBond.Application.Interfaces.Service;

public interface ICurrentService
{
    Guid UserId { get; }
    bool IsAdmin { get; }
    bool IsInRole(string role);
    string? UserName { get; }
}