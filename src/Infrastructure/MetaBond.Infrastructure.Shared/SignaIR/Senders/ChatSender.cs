using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.DTOs.Account.Message;
using MetaBond.Application.Interfaces.Service.SignaIR.Hubs;
using MetaBond.Application.Interfaces.Service.SignaIR.Senders;
using MetaBond.Infrastructure.Shared.SignaIR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MetaBond.Infrastructure.Shared.SignaIR.Senders;

public class ChatSender(IHubContext<ChatHub, IChatHub> hubContext) : IChatSender
{
    public async Task SendCreateChatAGroupAsync(ChatGroupDTos chatGroup)
    {
        await hubContext.Clients.Group(chatGroup.ChatId.ToString())
            .OnCreatedGroupChat(chatGroup);
    }

    public async Task SendCreateChatPrivateAsync(ChatPrivateDTos chatPrivate)
    {
        await hubContext.Clients.Group(chatPrivate.ChatId.ToString())
            .OnCreatedChatPrivate(chatPrivate);
    }

    public async Task SendJoinChatAsync(Guid chatId, ChatWithUserDTos chatWithUser)
    {
        await hubContext.Clients.Group(chatId.ToString())
            .OnJoinChat(chatWithUser);
    }

    public async Task SendLeaveChatAsync(Guid chatId, ChatWithUserDTos chatWithUser)
    {
        await hubContext.Clients.Group(chatId.ToString())
            .OnLeaveChat(chatWithUser);
    }

    public async Task SendMessageAsync(Guid chatId, MessageDto message)
    {
        await hubContext.Clients.Group(chatId.ToString())
            .OnMessageReceived(chatId, message.UserId, message.Content);
    }

    public async Task SendMessagesReadAsync(Guid chatId, Guid userId, IReadOnlyCollection<Guid> messageIds)
    {
        await hubContext.Clients.Group(chatId.ToString())
            .OnMessagesRead(chatId, userId, messageIds);
    }
}