using MediatR;
using MetaBond.Application.Feature.Chat.Commands.CreateChatGroup;
using MetaBond.Application.Feature.Chat.Commands.CreateChatPrivate;
using MetaBond.Application.Feature.Chat.Commands.JoinChat;
using MetaBond.Application.Feature.Chat.Commands.LeaveChat;
using MetaBond.Application.Feature.Messages.Commands.Create;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Interfaces.Service.SignaIR.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace MetaBond.Infrastructure.Shared.SignaIR.Hubs;

public class ChatHub(IMediator mediator, ICurrentService currentService) : Hub<IChatHub>
{
    public async Task CreateMessage(Guid chatId, string message)
    {
        await mediator.Send(new CreateMessageCommand()
            { SenderId = currentService.CurrentId, ChatId = chatId, Content = message });
    }

    // Create chat private method
    public async Task CreateChatPrivate()
    {
        await mediator.Send(new CreateChatPrivateCommand() { UserId = currentService.CurrentId });
    }

    //Create chat group method
    public async Task CreateChatGroup(Guid chatId, Guid communityId, string name, IFormFile? photo)
    {
        var command = new CreateChatGroupCommand()
        {
            UserId = currentService.CurrentId,
            CommunityId = communityId,
            Name = name,
            Photo = photo
        };

        await mediator.Send(command, CancellationToken.None);
    }

    //Create join chat method
    public async Task JoinChat(Guid chatId)
    {
        var command = new JoinGroupChatCommand()
        {
            ChatId = chatId,
            UserId = currentService.CurrentId
        };

        await mediator.Send(command, CancellationToken.None);
    }

    // Create leave chat method
    public async Task LeaveChat(Guid chatId)
    {
        var command = new LeaveGroupCommand()
        {
            UserId = currentService.CurrentId,
            ChatId = chatId
        };

        await mediator.Send(command, CancellationToken.None);

        // Remove user from group
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
    }
}