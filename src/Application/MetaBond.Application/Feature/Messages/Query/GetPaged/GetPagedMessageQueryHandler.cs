using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Message;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Messages.Query.GetPaged;

internal sealed class GetPagedMessageQueryHandler(
    ILogger<GetPagedMessageQueryHandler> logger,
    IMessageRepository messageRepository,
    IChatRepository chatRepository,
    IDistributedCache cache
) : IQueryHandler<GetPagedMessageQuery, PagedResult<MessageWithUserDTos>>
{
    public async Task<ResultT<PagedResult<MessageWithUserDTos>>> Handle(GetPagedMessageQuery request,
        CancellationToken cancellationToken)
    {
        var chat = await EntityHelper.GetEntityByIdAsync(
            chatRepository.GetByIdAsync,
            request.ChatId,
            "Chat",
            logger);

        if (!chat.IsSuccess) return ResultT<PagedResult<MessageWithUserDTos>>.Failure(chat.Error!);

        var key = $"chat-{request.ChatId}-messages-{request.PageNumber}-{request.PageSize}";

        var result = await cache.GetOrCreateAsync(key, async () =>
        {
            var pagedResult = await messageRepository.GetPagedMessagesAsync(request.PageNumber, request.PageSize,
                request.ChatId, cancellationToken);

            var items = pagedResult.Items!.ToList();

            var messageDto = items.Select(m => MessageMapper.MessageWithUserDTos(m, m.Sender!)).ToList();

            PagedResult<MessageWithUserDTos> pagedResultDto = new()
            {
                CurrentPage = pagedResult.CurrentPage,
                Items = messageDto,
                TotalItems = pagedResult.TotalItems,
                TotalPages = pagedResult.TotalPages
            };

            return pagedResultDto;
        }, cancellationToken: cancellationToken);

        if (result.Items != null && !result.Items.Any())
        {
            logger.LogWarning("No messages found for ChatId {ChatId}. Returning 'Chat not found' error.",
                request.ChatId);

            return ResultT<PagedResult<MessageWithUserDTos>>.Failure(Error.NotFound("404", "Chat not found"));
        }

        logger.LogInformation("Chat messages retrieved successfully");

        return ResultT<PagedResult<MessageWithUserDTos>>.Success(result);
    }
}