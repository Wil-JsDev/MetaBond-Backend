using System.Security.Claims;
using MetaBond.Application.DTOs.GitHub;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Interfaces.Service;

public interface IGitHubAuthService
{
    Task<ResultT<GitHubResponseDTos>> AuthenticatedGitHubAsync(ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken);
}