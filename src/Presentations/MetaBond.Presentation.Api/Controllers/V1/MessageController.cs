using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Account.Message;
using MetaBond.Application.Feature.Messages.Query.GetPaged;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/messages")]
[Authorize]
public class MessageController(IMediator mediator) : ControllerBase
{
    [HttpGet("{chatId}/chats")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get message by chat id",
        Description = "Get message by chat id"
    )]
    public async Task<ResultT<PagedResult<MessageWithUserDTos>>> GetPagedMessageByChatIdAsync([FromRoute] Guid chatId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetPagedMessageQuery()
        {
            ChatId = chatId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }
}