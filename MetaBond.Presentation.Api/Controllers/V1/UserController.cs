using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Account.Password;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Feature.User.Commands.ConfirmAccount;
using MetaBond.Application.Feature.User.Commands.Create;
using MetaBond.Application.Feature.User.Commands.ForgotPassword;
using MetaBond.Application.Feature.User.Commands.Update;
using MetaBond.Application.Feature.User.Commands.UpdatePassword;
using MetaBond.Application.Feature.User.Commands.UpdatePhoto;
using MetaBond.Application.Feature.User.Query.GetByUsername;
using MetaBond.Application.Feature.User.Query.GetUserWithFriendship;
using MetaBond.Application.Feature.User.Query.Pagination;
using MetaBond.Application.Feature.User.Query.SearchByUsername;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/users")]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [DisableRateLimiting]
    public async Task<ResultT<UserDTos>> CreateAsync([FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPost("confirm-account")]
    [DisableRateLimiting]
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

    [HttpPost("forgot-password")]
    [DisableRateLimiting]
    public async Task<ResultT<string>> ForgotPasswordAsync([FromQuery] string email,
        CancellationToken cancellationToken)
    {
        ForgotPasswordUserCommand command = new()
        {
            Email = email
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut]
    [DisableRateLimiting]
    public async Task<ResultT<UpdateUserDTos>> UpdateAsync([FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut("{code}")]
    [DisableRateLimiting]
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
    [DisableRateLimiting]
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
    [EnableRateLimiting("fixed")]
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
    [EnableRateLimiting("fixed")]
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
    [EnableRateLimiting("fixed")]
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
    [EnableRateLimiting("fixed")]
    public async Task<ResultT<IEnumerable<UserDTos>>> GetUsernameAsync([FromQuery] string username,
        CancellationToken cancellationToken)
    {
        SearchByUsernameUserQuery query = new()
        {
            Username = username
        };

        return await mediator.Send(query, cancellationToken);
    }
}