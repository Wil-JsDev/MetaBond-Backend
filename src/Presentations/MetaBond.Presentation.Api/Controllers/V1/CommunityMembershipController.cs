using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Account.CommunityMembership;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Feature.CommunityMembership.Commands.JoinCommunity;
using MetaBond.Application.Feature.CommunityMembership.Commands.LeaveCommunity;
using MetaBond.Application.Feature.CommunityMembership.Commands.UpdateRole;
using MetaBond.Application.Feature.CommunityMembership.Query.GetCommunityMember;
using MetaBond.Application.Feature.CommunityMembership.Query.GetUserCommunities;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v:{version:ApiVersion}/community-memberships")]
public class CommunityMembershipController(IMediator mediator, ICurrentService currentService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Join a community",
        Description = "Allows a user to join a specific community."
    )]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ResultT<CommunityMembershipDto>> JoinCommunityAsync(
        [FromBody] JoinCommunityCommand joinCommunityCommand,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(joinCommunityCommand, cancellationToken);
    }

    [HttpPut("{communityId}/members/users/role")]
    [Authorize(Roles =
        $"{CommunityMembershipRoleNames.Owner},{CommunityMembershipRoleNames.Moderator},{UserRoleNames.Admin}")]
    [SwaggerOperation(
        Summary = "Update user role in a community",
        Description = "Updates the role of a user within a community."
    )]
    public async Task<ResultT<string>> UpdateRoleAsync([FromQuery] Guid communityId, [FromQuery] Guid userId,
        [FromBody] CommunityMembershipRoleParameter roles, CancellationToken cancellationToken)
    {
        var command = new UpdateCommunityMembershipRoleCommand
        {
            UserId = currentService.CurrentId,
            CommunityId = communityId,
            Role = roles.Role
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpPatch("leave")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Leave a community",
        Description = "Allows a user to leave a community."
    )]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ResultT<LeaveCommunityDto>> LeaveCommunityAsync([FromQuery] Guid communityId,
        CancellationToken cancellationToken)
    {
        var command = new LeaveCommunityCommand
        {
            UserId = currentService.CurrentId,
            CommunityId = communityId
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpGet("community/{communityId}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get community members",
        Description = "Retrieves a paginated list of members from a specific community."
    )]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ResultT<PagedResult<CommunityMembersDto>>> GetCommunityMemberAsync(
        [FromRoute] Guid communityId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetCommunityMemberQuery
        {
            CommunityId = communityId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("me/commuities")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get user communities",
        Description = "Retrieves a paginated list of communities that a specific user belongs to."
    )]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ResultT<PagedResult<CommunitiesDTos>>> GetCommunityAsync(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetUserCommunitiesQuery
        {
            UserId = currentService.CurrentId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }
}