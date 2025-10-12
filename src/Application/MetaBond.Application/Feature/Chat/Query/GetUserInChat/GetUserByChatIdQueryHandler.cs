using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Chat.Query.GetUserInChat;

internal sealed class GetUserByChatIdQueryHandler(
    ILogger<GetUserByChatIdQueryHandler> logger,
    IChatRepository chatRepository,
    IDistributedCache cache
) : IQueryHandler<GetUserByChatIdQuery, PagedResult<UserChatDTos>>
{
    public async Task<ResultT<PagedResult<UserChatDTos>>> Handle(GetUserByChatIdQuery request,
        CancellationToken cancellationToken)
    {
        var chat = await EntityHelper.GetEntityByIdAsync(
            chatRepository.GetByIdAsync,
            request.ChatId,
            "Chat",
            logger
        );

        if (!chat.IsSuccess) return ResultT<PagedResult<UserChatDTos>>.Failure(chat.Error!);

        if (!await chatRepository.HasParticipantsAsync(request.ChatId, cancellationToken))
        {
            logger.LogWarning("Chat {ChatId} has no participants.", request.ChatId);

            return ResultT<PagedResult<UserChatDTos>>.Failure(
                Error.Failure("404", "The requested chat does not have any active participants.")
            );
        }

        string key = $"chat-participants-{request.ChatId}";

        var result = await cache.GetOrCreateAsync(key, async () =>
        {
            var getUserInChat = await chatRepository.GetUsersInChatAsync(request.ChatId, request.PageSize,
                request.PageNumber, cancellationToken);

            var items = getUserInChat.Items ?? [];

            var userInChatDTos = items.Select(UserMapper.MapToUserChatDTos);

            var pagedResult = new PagedResult<UserChatDTos>
            {
                CurrentPage = getUserInChat.CurrentPage,
                Items = userInChatDTos,
                TotalItems = getUserInChat.TotalItems,
                TotalPages = getUserInChat.TotalPages
            };

            return pagedResult;
        }, cancellationToken: cancellationToken);

        if (!result.Items.Any())
        {
            logger.LogWarning("No users found for chat with ID: {ChatId}", request.ChatId);

            return ResultT<PagedResult<UserChatDTos>>.Failure(
                Error.Failure("404", "No participants found for the specified chat.")
            );
        }

        logger.LogInformation("Users found for chat with ID: {ChatId}", request.ChatId);

        return ResultT<PagedResult<UserChatDTos>>.Success(result);
    }
}