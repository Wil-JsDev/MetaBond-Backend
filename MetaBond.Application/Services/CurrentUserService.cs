using System.Security.Claims;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace MetaBond.Application.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentService
{
    public Guid UserId
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

    public string? UserName => httpContextAccessor.HttpContext?.User.Identity?.Name;
}