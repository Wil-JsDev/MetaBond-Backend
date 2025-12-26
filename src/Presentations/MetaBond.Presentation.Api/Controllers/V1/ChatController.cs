using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.Feature.Chat.Query.GetChatByUserId;
using MetaBond.Application.Feature.Chat.Query.GetChatWithLastMessage;
using MetaBond.Application.Feature.Chat.Query.GetGroupChatsByCommunity;
using MetaBond.Application.Feature.Chat.Query.GetPagedChatByUser;
using MetaBond.Application.Feature.Chat.Query.GetUserInChat;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/chats")]
public class ChatController(IMediator mediator, ICurrentService currentService) : ControllerBase
{
    [HttpGet("users")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get chat by user id",
        Description = "Gets a list of chats for a user."
    )]
    public async Task<ResultT<PagedResult<ChatDTos>>> GetChatByUserId([FromRoute] Guid userId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedChatByUserQuery()
        {
            UserId = currentService.CurrentId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{chatId}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get specific chat details for an authorized user",
        Description =
            "Retrieves a chat (group or private) by its ID, including all messages and ensuring the authenticated user is a participant. Used for loading a chat view."
    )]
    public async Task<ResultT<ChatBaseWithMessageDTos>> GetChatDetailsById([FromRoute] Guid chatId,
        CancellationToken cancellationToken)
    {
        var query = new GetChatByUserIdQuery()
        {
            UserId = currentService.CurrentId,
            ChatId = chatId
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("users/last-message")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get chats with last message",
        Description = "Retrieves a list of chats with the last message for the authenticated user."
    )]
    public async Task<ResultT<PagedResult<ChatWitLastMessageDTos>>> GetChatsWithLastMessage(
        [FromQuery] int pageNumber, [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetChatWithLastMessageQuery
        {
            UserId = currentService.CurrentId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("groups/{communityId}/communities")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get group chats by community ID",
        Description = "Retrieves a paginated list of group chats associated with a specific community."
    )]
    public async Task<ResultT<PagedResult<ChatDTos>>> GetChatsGroupByCommunityId([FromRoute] Guid communityId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken
    )
    {
        var query = new GetGroupChatsByCommunityQuery
        {
            CommunityId = communityId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{chatId}/users")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated participants for a specific chat",
        Description =
            "Retrieves a paginated list of users participating in the specified chat. Requires authentication and is rate-limited."
    )]
    public async Task<ResultT<PagedResult<UserChatDTos>>> GetUserByChatIdAsync(
        [FromRoute] Guid chatId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = new GetUserByChatIdQuery()
        {
            ChatId = chatId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }
}