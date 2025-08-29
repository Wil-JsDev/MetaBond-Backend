using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.CommunityMembership.Commands.JoinCommunity;
using MetaBond.Application.Feature.CommunityMembership.Commands.LeaveCommunity;
using MetaBond.Application.Feature.CommunityMembership.Commands.UpdateRole;
using MetaBond.Application.Feature.CommunityMembership.Query.GetCommunityMember;
using MetaBond.Application.Feature.CommunityMembership.Query.GetUserCommunities;
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
    public async Task<IActionResult> JoinCommunityAsync([FromBody] JoinCommunityCommand joinCommunityCommand,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(joinCommunityCommand, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPatch("role")]
    [SwaggerOperation(Description = "Updates the role of a user within a community.")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> UpdateRoleAsync([FromQuery] Guid communityId, [FromQuery] Guid userId,
        [FromBody] string roles, CancellationToken cancellationToken)
    {
        var command = new UpdateCommunityMembershipRoleCommand
        {
            UserId = userId,
            CommunityId = communityId,
            Role = roles
        };

        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPatch("leave")]
    [SwaggerOperation(Description = "Allows a user to leave a community.")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> LeaveCommunityAsync([FromQuery] Guid communityId, [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new LeaveCommunityCommand
        {
            UserId = userId,
            CommunityId = communityId
        };

        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("community/{communityId}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(Description = "Retrieves a paginated list of members from a specific community.")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> GetCommunityMemberAsync([FromRoute] Guid communityId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetCommunityMemberQuery
        {
            CommunityId = communityId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("users/{userId}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(Description = "Retrieves a paginated list of communities that a specific user belongs to.")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> GetCommunityAsync([FromRoute] Guid userId, [FromQuery] int pageNumber,
        [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetUserCommunitiesQuery
        {
            UserId = userId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}