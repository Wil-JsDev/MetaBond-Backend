using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Account.CommunityMembership;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Feature.CommunityMembership.Commands.JoinCommunity;
using MetaBond.Application.Feature.CommunityMembership.Commands.LeaveCommunity;
using MetaBond.Application.Feature.CommunityMembership.Commands.UpdateRole;
using MetaBond.Application.Feature.CommunityMembership.Query.GetCommunityMember;
using MetaBond.Application.Feature.CommunityMembership.Query.GetUserCommunities;
using MetaBond.Application.Helpers;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v:{version:ApiVersion}/community-memberships")]
public class CommunityMembershipController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Description = "Allows a user to join a specific community.")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ResultT<CommunityMembershipDto>> JoinCommunityAsync(
        [FromBody] JoinCommunityCommand joinCommunityCommand,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(joinCommunityCommand, cancellationToken);
    }

    [HttpPatch("role")]
    [SwaggerOperation(Description = "Updates the role of a user within a community.")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ResultT<string>> UpdateRoleAsync([FromQuery] Guid communityId, [FromQuery] Guid userId,
        [FromBody] string roles, CancellationToken cancellationToken)
    {
        var command = new UpdateCommunityMembershipRoleCommand
        {
            UserId = userId,
            CommunityId = communityId,
            Role = roles
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpPatch("leave")]
    [SwaggerOperation(Description = "Allows a user to leave a community.")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ResultT<LeaveCommunityDto>> LeaveCommunityAsync([FromQuery] Guid communityId,
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new LeaveCommunityCommand
        {
            UserId = userId,
            CommunityId = communityId
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpGet("community/{communityId}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(Description = "Retrieves a paginated list of members from a specific community.")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ResultT<PagedResult<CommunityMembersDto>>> GetCommunityMemberAsync([FromRoute] Guid communityId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetCommunityMemberQuery
        {
            CommunityId = communityId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("users/{userId}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(Description = "Retrieves a paginated list of communities that a specific user belongs to.")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ResultT<PagedResult<CommunitiesDTos>>> GetCommunityAsync([FromRoute] Guid userId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetUserCommunitiesQuery
        {
            UserId = userId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }
}