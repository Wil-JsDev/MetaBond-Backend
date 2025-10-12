using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service.SignaIR.Senders;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MetaBond.Application.Feature.Chat.Commands.JoinChat;

internal sealed class JoinGroupChatCommandHandler(
    ILogger<JoinGroupChatCommandHandler> logger,
    IChatRepository chatRepository,
    IUserChatRepository userChatRepository,
    IUserRepository userRepository,
    IChatSender chatSender
) : ICommandHandler<JoinGroupChatCommand, ChatWithUserDTos>
{
    public async Task<ResultT<ChatWithUserDTos>> Handle(JoinGroupChatCommand request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId,
            "User",
            logger);

        if (!user.IsSuccess) return ResultT<ChatWithUserDTos>.Failure(user.Error!);

        var chat = await EntityHelper.GetEntityByIdAsync(
            chatRepository.GetByIdAsync,
            request.ChatId,
            "Chat",
            logger
        );

        if (!chat.IsSuccess) return ResultT<ChatWithUserDTos>.Failure(chat.Error!);

        if (chat.Value.Type != ChatType.CommunityGroup.ToString())
            return ResultT<ChatWithUserDTos>.Failure(Error.Forbidden("403", "Cannot join a non-group chat."));

        var userChat = new UserChat()
        {
            ChatId = request.ChatId,
            UserId = request.UserId
        };

        await userChatRepository.AddUserToChatAsync(userChat, cancellationToken);

        logger.LogInformation("User added to chat successfully with ID: {ChatId}", request.ChatId);

        var chatWithUserDto = ChatMapper.MapToChatWithUserDTos(chat.Value, user.Value);

        await chatSender.SendJoinChatAsync(request.ChatId, chatWithUserDto);

        logger.LogInformation("Chat sent successfully with ID: {ChatId}", chatWithUserDto.ChatId);

        return ResultT<ChatWithUserDTos>.Success(chatWithUserDto);
    }
}