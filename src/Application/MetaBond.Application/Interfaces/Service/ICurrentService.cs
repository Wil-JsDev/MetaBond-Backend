namespace MetaBond.Application.Interfaces.Service;

public interface ICurrentService
{
    Guid CurrentId { get; }
    bool IsAdmin { get; }
    bool IsInRole(string role);
    string? UserName { get; }
    IEnumerable<string> GetRoles { get; }
}