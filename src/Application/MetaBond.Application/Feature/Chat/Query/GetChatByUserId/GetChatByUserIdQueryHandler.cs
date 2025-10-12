using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MetaBond.Application.Feature.Chat.Query.GetChatByUserId;

internal sealed class GetChatByUserIdQueryHandler(
    ILogger<GetChatByUserIdQueryHandler> logger,
    IChatRepository chatRepository,
    IUserChatRepository userChatRepository,
    IUserRepository userRepository,
    IDistributedCache cache
) : IQueryHandler<GetChatByUserIdQuery, ChatBaseWithMessageDTos>
{
    public async Task<ResultT<ChatBaseWithMessageDTos>> Handle(GetChatByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId,
            "User",
            logger
        );

        if (!user.IsSuccess) return ResultT<ChatBaseWithMessageDTos>.Failure(user.Error!);

        var chat = await EntityHelper.GetEntityByIdAsync(
            chatRepository.GetByIdAsync,
            request.ChatId,
            "Chat",
            logger
        );

        if (!chat.IsSuccess) return ResultT<ChatBaseWithMessageDTos>.Failure(chat.Error!);

        var chatByUser = await chatRepository.GetByIdAndUserIdAsync(request.ChatId, request.UserId, cancellationToken);

        if (chatByUser is null)
            return ResultT<ChatBaseWithMessageDTos>.Failure(Error.Failure("404", "Chat not found."));

        string key = $"chat-by-user-{request.UserId}-{request.ChatId}";

        var result = await cache.GetOrCreateAsync(key, () =>
        {
            var participants = chatByUser.UserChats;

            var messages = chatByUser.Messages.OrderBy(ms => ms.SentAt);

            ChatBaseWithMessageDTos finalDto;


            if (participants.Count >= 2 || chat.Value.Type == ChatType.CommunityGroup.ToString())
            {
                finalDto = ChatMapper.MapToChatGroupWithMessageDTos(chat.Value, messages);
            }
            else
            {
                finalDto = ChatMapper.MapToChatPrivateWithMessageDTos(chat.Value, messages);
            }

            return Task.FromResult(finalDto);
        }, cancellationToken: cancellationToken);

        logger.LogInformation("Chat sent successfully with ID: {ChatId}", result.ChatId);

        return ResultT<ChatBaseWithMessageDTos>.Success(result);
    }
}