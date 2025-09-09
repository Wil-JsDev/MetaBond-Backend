using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Interest;
using MetaBond.Application.DTOs.InterestCategory;
using MetaBond.Application.Feature.Interest.Commands.Create;
using MetaBond.Application.Feature.InterestCategory.Command.Create;
using MetaBond.Application.Feature.InterestCategory.Command.Update;
using MetaBond.Application.Feature.InterestCategory.Query.GetById;
using MetaBond.Application.Feature.InterestCategory.Query.GetByName;
using MetaBond.Application.Feature.InterestCategory.Query.Pagination;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.AspNetCore.Mvc;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/interest-categories")]
public class InterestCategoryController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ResultT<InterestCategoryDTos>> CreateAsync([FromBody] CreateInterestCategoryCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut("{interestCategoryId}")]
    public async Task<ResultT<UpdateInterestCategoryDTos>> UpdateAsync([FromRoute] Guid interestCategoryId,
        [FromBody] ParameterInterestCategory interestCategory, CancellationToken cancellationToken)
    {
        var command = new UpdateInterestCategoryCommand()
        {
            Name = interestCategory.Name,
            InterestCategoryId = interestCategoryId
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpGet("{interestCategoryId}")]
    public async Task<ResultT<InterestCategoryGeneralDTos>> GetByIdAsync([FromRoute] Guid interestCategoryId,
        CancellationToken cancellationToken)
    {
        var query = new GetByIdInterestCategoryQuery
        {
            InterestCategoryId = interestCategoryId
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("search/{name}")]
    public async Task<ResultT<InterestCategoryGeneralDTos>> GetByNameAsync([FromRoute] string name,
        CancellationToken cancellationToken)
    {
        var query = new GetByNameInterestCategoryQuery()
        {
            Name = name
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("pagination")]
    public async Task<ResultT<PagedResult<InterestCategoryGeneralDTos>>> GetPagedAsync(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = new GetPagedInterestCategoryQuery()
        {
            PageSize = pageSize,
            PageNumber = pageNumber
        };

        return await mediator.Send(query, cancellationToken);
    }
}