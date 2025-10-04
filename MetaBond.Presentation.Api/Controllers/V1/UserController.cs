using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Account.Password;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Feature.User.Commands.ConfirmAccount;
using MetaBond.Application.Feature.User.Commands.Create;
using MetaBond.Application.Feature.User.Commands.ForgotPassword;
using MetaBond.Application.Feature.User.Commands.ResetPassword;
using MetaBond.Application.Feature.User.Commands.Update;
using MetaBond.Application.Feature.User.Commands.UpdatePassword;
using MetaBond.Application.Feature.User.Commands.UpdatePhoto;
using MetaBond.Application.Feature.User.Query.GetByUsername;
using MetaBond.Application.Feature.User.Query.GetUserWithFriendship;
using MetaBond.Application.Feature.User.Query.Pagination;
using MetaBond.Application.Feature.User.Query.SearchByUsername;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/users")]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Create a new user",
        Description = "Creates a new user using the provided command data."
    )]
    public async Task<ResultT<UserDTos>> CreateAsync([FromForm] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPost("confirm-account")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Confirm user account",
        Description = "Confirms a user account using the provided userId and confirmation code."
    )]
    public async Task<ResultT<string>> ConfirmAccountAsync([FromQuery] Guid userId, [FromQuery] string code,
        CancellationToken cancellationToken)
    {
        ConfirmAccountCommand command = new()
        {
            UserId = userId,
            Code = code
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpPost("{userId}/forgot-password")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Forgot password",
        Description = "Sends a password reset email to the user with the provided email address."
    )]
    public async Task<ResultT<string>> ForgotPasswordAsync([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        ForgotPasswordUserCommand command = new()
        {
            UserId = userId
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Update user information",
        Description = "Updates the information of an existing user."
    )]
    public async Task<ResultT<UpdateUserDTos>> UpdateAsync([FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut("{code}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Update user password",
        Description = "Updates the password for a user using a reset code and new password information."
    )]
    public async Task<ResultT<string>> UpdatePasswordAsync([FromRoute] string code, [FromQuery] string email,
        [FromBody] UpdatePasswordParameter parameter, CancellationToken cancellationToken)
    {
        UpdatePasswordUserCommand passwordUserCommand = new()
        {
            Code = code,
            ConfirmNewPassword = parameter.NewPasswordConfirmPassword,
            Email = email,
            NewPassword = parameter.NewPassword
        };

        return await mediator.Send(passwordUserCommand, cancellationToken);
    }

    [HttpPut("{userId}/photo")]
    [Authorize(Roles = UserRoleNames.User)]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Update user photo",
        Description = "Updates the profile photo of a user by their unique identifier."
    )]
    public async Task<ResultT<string>> UpdatePhotoAsync([FromRoute] Guid userId, UpdatePhotoParameterDto parameter,
        CancellationToken cancellationToken)
    {
        UpdatePhotoUserCommand command = new()
        {
            ImageFile = parameter.Photo,
            UserId = userId
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpGet("search-by/{username}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get user by username",
        Description = "Retrieves a user by their username."
    )]
    public async Task<ResultT<UserDTos>> GetByUsernameAsync([FromRoute] string username,
        CancellationToken cancellationToken)
    {
        GetByUsernameUserQuery usernameUserQuery = new()
        {
            Username = username
        };

        return await mediator.Send(usernameUserQuery, cancellationToken);
    }

    [HttpGet("{userId}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get user with friendship info",
        Description = "Retrieves a user and their friendship information by user ID."
    )]
    public async Task<ResultT<UserWithFriendshipDTos>> GetUserWithFriendshipAsync([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        GetUserWithFriendshipByIdQuery query = new()
        {
            UserId = userId
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("pagination")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated users",
        Description = "Retrieves a paginated list of users."
    )]
    public async Task<ResultT<PagedResult<UserDTos>>> GetPagedAsync([FromQuery] int pageNumber,
        [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        GetPagedUserQuery userQuery = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(userQuery, cancellationToken);
    }

    [HttpGet("search")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Search users by username",
        Description = "Retrieves a paginated list of users filtered by username."
    )]
    public async Task<ResultT<PagedResult<UserDTos>>> GetUsernameAsync([FromQuery] string username,
        [FromQuery] int pageNumber, [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        SearchByUsernameUserQuery query = new()
        {
            Username = username,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpPut("reset-password/{userId}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Reset user password",
        Description =
            "Resets the password for a user by their unique identifier. Requires the new password and confirmation."
    )]
    public async Task<ResultT<string>> ResetPasswordAsync([FromRoute] Guid userId,
        [FromBody] UpdatePasswordParameter request, CancellationToken cancellationToken)
    {
        ResetPasswordUserCommand command = new()
        {
            UserId = userId,
            NewPassword = request.NewPassword,
            ConfirmNewPassword = request.NewPasswordConfirmPassword
        };

        return await mediator.Send(command, cancellationToken);
    }
}