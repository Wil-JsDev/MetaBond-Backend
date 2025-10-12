using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Chat.Query.GetChatWithLastMessage;

internal sealed class GetChatWithLastMessageQueryHandler(
    ILogger<GetChatWithLastMessageQueryHandler> logger,
    IUserChatRepository userChatRepository,
    IUserRepository userRepository,
    IChatRepository chatRepository,
    IDistributedCache cache
) : IQueryHandler<GetChatWithLastMessageQuery, PagedResult<ChatWitLastMessageDTos>>
{
    public async Task<ResultT<PagedResult<ChatWitLastMessageDTos>>> Handle(GetChatWithLastMessageQuery request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId,
            "User",
            logger);

        if (!user.IsSuccess) return ResultT<PagedResult<ChatWitLastMessageDTos>>.Failure(user.Error!);

        var validationPagination =
            PaginationHelper.ValidatePagination<GetChatWithLastMessageQuery>(request.PageNumber, request.PageSize,
                logger);

        if (!validationPagination.IsSuccess)
            return ResultT<PagedResult<ChatWitLastMessageDTos>>.Failure(validationPagination.Error);

        if (!await chatRepository.HasAnyChatAsync(request.UserId, cancellationToken))
        {
            logger.LogWarning("No chats found for user with ID: {UserId}", request.UserId);

            return ResultT<PagedResult<ChatWitLastMessageDTos>>.Failure(
                Error.Failure("404", "The user does not have any active chats or conversations.")
            );
        }

        string key = $"chats-with-last-message-{request.UserId}-{request.PageNumber}-{request.PageSize}";

        var result = await cache.GetOrCreateAsync(key, async () =>
        {
            var chatsWithLastMessage = await chatRepository.GetChatsWithLastMessageAsync(request.UserId,
                request.PageNumber, request.PageSize, cancellationToken);

            var items = chatsWithLastMessage.Items ?? [];

            var chatsDtos = items.Select(ChatMapper.MapToChatWitLastMessageDTos);

            var pagedResult = new PagedResult<ChatWitLastMessageDTos>
            {
                CurrentPage = chatsWithLastMessage.CurrentPage,
                Items = chatsDtos,
                TotalItems = chatsWithLastMessage.TotalItems,
                TotalPages = chatsWithLastMessage.TotalPages
            };

            return pagedResult;
        }, cancellationToken: cancellationToken);

        if (!result.Items.Any())
        {
            logger.LogWarning("No chats found for user with ID: {UserId}", request.UserId);

            return ResultT<PagedResult<ChatWitLastMessageDTos>>.Failure(
                Error.Failure("404", "No chats found for user with ID"));
        }

        logger.LogInformation("Chats found for user with ID: {UserId}", request.UserId);

        return ResultT<PagedResult<ChatWitLastMessageDTos>>.Success(result);
    }
}