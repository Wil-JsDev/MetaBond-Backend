using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.Factories;
using MetaBond.Application.Factories.Parameters;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Chat.Commands.CreateChatPrivate;

internal sealed class CreateChatPrivateCommandHandler(
    ILogger<CreateChatPrivateCommandHandler> logger,
    IChatRepository chatRepository,
    IUserChatRepository userChatRepository,
    IUserRepository userRepository
) : ICommandHandler<CreateChatPrivateCommand, ChatPrivateDTos>
{
    public async Task<ResultT<ChatPrivateDTos>> Handle(CreateChatPrivateCommand request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userChatRepository.GetByIdAsync,
            request.UserId,
            "User",
            logger
        );

        if (!user.IsSuccess) return ResultT<ChatPrivateDTos>.Failure(user.Error!);

        var chat = ChatFactory.CreateChat(new CreateChatOptions()
        {
            Type = ChatType.Direct
        });

        await chatRepository.CreateAsync(chat.Value, cancellationToken);

        logger.LogInformation("Chat created successfully with ID: {ChatId}", chat.Value.Id);

        var userChat = new UserChat()
        {
            ChatId = chat.Value.Id,
            UserId = request.UserId
        };

        await userChatRepository.CreateAsync(userChat, cancellationToken);

        logger.LogInformation("Chat sent successfully with ID: {ChatId}", chat.Value.Id);

        var chatPrivateDto = ChatMapper.MapToChatPrivateDTos(chat.Value);

        return ResultT<ChatPrivateDTos>.Success(chatPrivateDto);
    }
}