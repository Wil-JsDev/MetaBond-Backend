using System.Security.Claims;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace MetaBond.Infrastructure.Shared.Service;

public class CurrentService(IHttpContextAccessor httpContextAccessor) : ICurrentService
{
    public Guid CurrentId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;

            return string.IsNullOrWhiteSpace(claim)
                ? throw new UnauthorizedAccessException("UserId not found in JWT claims.")
                : Guid.Parse(claim);
        }
    }

    public bool IsAdmin => IsInRole(UserRoleNames.Admin);

    public bool IsInRole(string role)
    {
        return httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
    }

    public string? UserName => httpContextAccessor.HttpContext?.User.FindFirst("username")?.Value;

    public IEnumerable<string> GetRoles =>
        httpContextAccessor
            .HttpContext?.User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
        ?? [];
}