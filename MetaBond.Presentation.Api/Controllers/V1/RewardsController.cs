using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.Rewards.Commands.Create;
using MetaBond.Application.Feature.Rewards.Commands.Delete;
using MetaBond.Application.Feature.Rewards.Commands.Update;
using MetaBond.Application.Feature.Rewards.Query.GetById;
using MetaBond.Application.Feature.Rewards.Query.GetCount;
using MetaBond.Application.Feature.Rewards.Query.GetRange;
using MetaBond.Application.Feature.Rewards.Query.GetRecent;
using MetaBond.Application.Feature.Rewards.Query.GetTop;
using MetaBond.Application.Feature.Rewards.Query.Pagination;
using MetaBond.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MetaBond.Presentation.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:ApiVersion}/rewards")]
    public class RewardsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RewardsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> AddAsync([FromBody] CreateRewardsCommand rewardsCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(rewardsCommand, cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new DeleteRewardsCommand { RewardsId = id };

            var result = await _mediator.Send(query,cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateRewardsCommand rewardsCommand,CancellationToken cancellationToken)
        {
            rewardsCommand.RewardsId = id;

            var result = await _mediator.Send(rewardsCommand,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new GetByIdRewardsQuery { RewardsId= id };

            var result = await _mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("count")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetCountByIdAsync(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCountRewardsQuery(),cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("filter-by-date-range")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetDateRangeAsync([FromQuery] DateRangeType range, CancellationToken cancellationToken)
        {
            var query = new GetByDateRangeRewardQuery { Range = range };

            var result = await _mediator.Send(query, cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("filter/recent")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetRecentAsync(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMostRecentRewardsQuery(),cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("top-rewards-by-count")]
        [DisableRateLimiting]
        public async Task<IActionResult> GetTopRewards([FromQuery] int count,CancellationToken cancellationToken)
        {
            var query = new GetTopRewardsQuery { TopCount = count };

            var result = await _mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("pagination")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetTopRewards([FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var query = new GetPagedRewardsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}
