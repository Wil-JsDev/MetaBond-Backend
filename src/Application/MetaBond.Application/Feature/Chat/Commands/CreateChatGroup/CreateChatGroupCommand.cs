using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;
using Microsoft.AspNetCore.Http;

namespace MetaBond.Application.Feature.Chat.Commands.CreateChatGroup;

public sealed class CreateChatGroupCommand : ICommand<ChatGroupDTos>
{
    public Guid UserId { get; set; }
    public Guid CommunityId { get; set; }
    public string? Name { get; set; }

    public IFormFile? Photo { get; set; }
}