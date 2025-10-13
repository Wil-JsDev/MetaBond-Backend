using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service.SignaIR.Senders;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Chat.Commands.LeaveChat;

internal sealed class LeaveGroupCommandHandler(
    ILogger<LeaveGroupCommandHandler> logger,
    IUserChatRepository userChatRepository,
    IUserRepository userRepository,
    IChatRepository chatRepository,
    IChatSender chatSender
) : ICommandHandler<LeaveGroupCommand, LeaveChatDTos>
{
    public async Task<ResultT<LeaveChatDTos>> Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId,
            "User",
            logger
        );

        if (!user.IsSuccess) return ResultT<LeaveChatDTos>.Failure(user.Error!);

        var chat = await EntityHelper.GetEntityByIdAsync(
            chatRepository.GetByIdAsync,
            request.ChatId,
            "Chat",
            logger
        );

        if (!chat.IsSuccess) return ResultT<LeaveChatDTos>.Failure(chat.Error!);

        if (chat.Value.Type == ChatType.CommunityGroup.ToString())
            return ResultT<LeaveChatDTos>.Failure(Error.Failure("400", "Cannot leave a group chat."));

        await userChatRepository.RemoveUserFromChatAsync(request.ChatId, request.UserId, cancellationToken);

        logger.LogInformation("User removed from chat successfully with ID: {ChatId}", request.ChatId);

        var leaveChatDto = ChatMapper.MapToLeaveChatDTos(chat.Value, user.Value);

        var notificationDto = new ChatWithUserDTos(
            request.ChatId,
            ChatType.CommunityGroup,
            chat.Value.Name,
            chat.Value.Photo,
            new UserChatDTos(request.UserId, null, null, null)
        );

        await chatSender.SendLeaveChatAsync(request.ChatId, notificationDto);

        return ResultT<LeaveChatDTos>.Success(leaveChatDto);
    }
}