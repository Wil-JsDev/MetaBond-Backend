using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Account.Auth;
using MetaBond.Application.Feature.Authentication.Commands.GenerateCommunityToken;
using MetaBond.Application.Feature.Authentication.Commands.LoginAdmin;
using MetaBond.Application.Feature.Authentication.Commands.LoginUser;
using MetaBond.Application.Feature.Authentication.Commands.RefreshTokenAdmin;
using MetaBond.Application.Feature.Authentication.Commands.RefreshTokenUser;
using MetaBond.Application.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("users")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Login a user",
        Description = "Authenticates a user and returns an access token and a refresh token."
    )]
    public async Task<ResultT<AuthenticationResponse>> LoginUserAsync(
        [FromBody] LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPost("admin")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Login an admin",
        Description = "Authenticates an admin and returns an access token and a refresh token."
    )]
    public async Task<ResultT<AuthenticationResponse>> LoginAdminAsync(
        [FromBody] LoginAdminCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPost("refresh-token/users")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Refresh a user token",
        Description = "Refreshes the access token and refresh token for a user using a valid refresh token."
    )]
    public async Task<ResultT<AuthenticationResponse>> RefreshTokenUserAsync(
        [FromBody] RefreshTokenUserCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPost("refresh-token/admin")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Refresh an admin token",
        Description = "Refreshes the access token and refresh token for an admin using a valid refresh token."
    )]
    public async Task<ResultT<AuthenticationResponse>> RefreshTokenAdminAsync(
        [FromBody] RefreshTokenAdminCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPost("communities/{communityId}/users/{userId}/token")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Generate community token",
        Description =
            "Generates a JWT for a specific user in a specific community, including the user's role in that community."
    )]
    public async Task<ResultT<AuthenticationCommunityResponse>> GenerateCommunityTokenAsync(
        [FromRoute] Guid communityId,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var command = new GenerateCommunityTokenCommand
        {
            CommunityId = communityId,
            UserId = userId
        };

        return await mediator.Send(command, cancellationToken);
    }
}