using System.Security.Claims;
using MetaBond.Application.DTOs.GitHub;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Interfaces.Service.Auth;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Domain.Models;

namespace MetaBond.Infrastructure.Shared.Service;

public class GitHubAuthService(
    IUserRepository userRepository,
    IJwtService jwtService,
    IRoleRepository roleRepository
) : IGitHubAuthService
{
    public async Task<ResultT<GitHubResponseDTos>> AuthenticatedGitHubAsync(ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var githubId = claimsPrincipal.FindFirst("urn:github:id")?.Value;
        var email = claimsPrincipal.FindFirst("urn:github:email")?.Value;
        var username = claimsPrincipal.FindFirst("urn:github:login")?.Value;
        var name = claimsPrincipal.FindFirst("urn:github:name")?.Value;
        var avatarUrl = claimsPrincipal.FindFirst("urn:github:avatar")?.Value;

        if (string.IsNullOrWhiteSpace(githubId) || string.IsNullOrWhiteSpace(email))
        {
            return ResultT<GitHubResponseDTos>.Failure(Error.Failure("400", "GitHub user information is missing."));
        }

        var existingUser = await userRepository.GetByIdGithubAsync(githubId, cancellationToken);

        if (existingUser is not null)
        {
            var existingAccessToken = jwtService.GenerateTokenUser(existingUser);
            var existingRefreshToken = jwtService.GenerateRefreshTokenUser(existingUser);

            return ResultT<GitHubResponseDTos>.Success(new GitHubResponseDTos(
                AccessToken: existingAccessToken,
                RefreshToken: existingRefreshToken,
                UserId: existingUser.Id
            ));
        }

        if (!string.IsNullOrEmpty(email))
        {
            var userWithEmail = await userRepository.GetByEmailAsync(email, cancellationToken);
            if (userWithEmail != null)
            {
                return ResultT<GitHubResponseDTos>.Failure(
                    Error.Conflict("409", "An account with this email already exists.")
                );
            }
        }

        var (firstName, lastName) = ParseName(name ?? username);

        var role = await roleRepository.GetByNameAsync(UserRoles.User.ToString(), cancellationToken);
        if (role is null)
        {
            return ResultT<GitHubResponseDTos>.Failure(Error.Failure("503", "Role not found."));
        }

        var user = new User()
        {
            Id = Guid.NewGuid(),
            GitHubId = githubId,
            Email = email ?? $"{username}@github.local",
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            Photo = avatarUrl,
            CreatedAt = DateTime.UtcNow,
            Password = null,
            RoleId = role.Id
        };

        await userRepository.CreateAsync(user, cancellationToken);

        var accessToken = jwtService.GenerateTokenUser(user);
        var refreshToken = jwtService.GenerateRefreshTokenUser(user);

        return ResultT<GitHubResponseDTos>.Success(new GitHubResponseDTos(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            UserId: user.Id
        ));
    }

    private static (string FirstName, string LastName) ParseName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return (string.Empty, string.Empty);

        var nameParts = fullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

        return nameParts.Length switch
        {
            0 => (string.Empty, string.Empty),
            1 => (nameParts[0], string.Empty),
            _ => (nameParts[0], nameParts[1])
        };
    }
}