using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Account.Admin;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Feature.Admin.Commands.BanUser;
using MetaBond.Application.Feature.Admin.Commands.ConfirmAccount;
using MetaBond.Application.Feature.Admin.Commands.Create;
using MetaBond.Application.Feature.Admin.Commands.UnbanUser;
using MetaBond.Application.Feature.Admin.Query.GetPagedUserStatus;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/admins")]
public class AdminController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new admin",
        Description = "Creates a new admin using the provided command data."
    )]
    [ProducesResponseType(typeof(ResultT<AdminDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultT<AdminDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultT<AdminDto>), StatusCodes.Status409Conflict)]
    public async Task<ResultT<AdminDto>> CreateAdminAsync([FromBody] CreateAdminCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPost("confirm-account")]
    [SwaggerOperation(
        Summary = "Confirm admin account",
        Description = "Confirms an admin account using the provided adminId and confirmation code."
    )]
    [ProducesResponseType(typeof(ResultT<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultT<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultT<string>), StatusCodes.Status404NotFound)]
    public async Task<ResultT<string>> ConfirmAccountAsync([FromQuery] Guid adminId, [FromQuery] string code,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmAccountAdminCommand()
        {
            AdminId = adminId,
            Code = code
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpPost("users/{userId}/ban")]
    [SwaggerOperation(
        Summary = "Ban a user",
        Description = "Bans a user by their unique identifier."
    )]
    [ProducesResponseType(typeof(ResultT<BanUserResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultT<BanUserResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultT<BanUserResultDto>), StatusCodes.Status404NotFound)]
    public async Task<ResultT<BanUserResultDto>> BanUserAsync([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new DisableUserCommand()
        {
            UserId = userId
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpPost("users/{userId}/unban")]
    [SwaggerOperation(
        Summary = "Unban a user",
        Description = "Unbans a user by their unique identifier."
    )]
    [ProducesResponseType(typeof(ResultT<UnbanUserResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultT<UnbanUserResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultT<UnbanUserResultDto>), StatusCodes.Status404NotFound)]
    public async Task<ResultT<UnbanUserResultDto>> UnBanUserAsync([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new UnBanUserCommand()
        {
            UserId = userId
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpGet("users/{statusAccount}")]
    [SwaggerOperation(
        Summary = "Get paged users by status",
        Description = "Retrieves a paginated list of users filtered by their account status."
    )]
    [ProducesResponseType(typeof(ResultT<PagedResult<UserDTos>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultT<PagedResult<UserDTos>>), StatusCodes.Status400BadRequest)]
    public async Task<ResultT<PagedResult<UserDTos>>> GetPagedUserStatusAccountAsync(
        [FromRoute] StatusAccount statusAccount,
        [FromQuery] int pageNumber, [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedUserStatusQuery()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            StatusAccount = statusAccount
        };

        return await mediator.Send(query, cancellationToken);
    }
}