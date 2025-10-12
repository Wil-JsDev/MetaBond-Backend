using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.Factories;
using MetaBond.Application.Factories.Parameters;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Interfaces.Service.SignaIR.Senders;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Chat.Commands.CreateChatGroup;

internal sealed class CreateChatGroupCommandHandler(
    ILogger<CreateChatGroupCommandHandler> logger,
    ICommunitiesRepository communitiesRepository,
    IChatRepository chatRepository,
    IUserChatRepository userChatRepository,
    IUserRepository userRepository,
    ICloudinaryService cloudinaryService,
    IChatSender chatSender
) : ICommandHandler<CreateChatGroupCommand, ChatGroupDTos>
{
    public async Task<ResultT<ChatGroupDTos>> Handle(CreateChatGroupCommand request,
        CancellationToken cancellationToken)
    {
        var community = await EntityHelper.GetEntityByIdAsync(
            communitiesRepository.GetByIdAsync,
            request.CommunityId,
            "Community",
            logger
        );

        if (!community.IsSuccess) return ResultT<ChatGroupDTos>.Failure(community.Error!);

        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId,
            "User",
            logger
        );

        if (!user.IsSuccess) return ResultT<ChatGroupDTos>.Failure(user.Error!);

        string? imageUrl = null;

        await using (var stream = request.Photo!.OpenReadStream())
        {
            imageUrl = await cloudinaryService.UploadImageCloudinaryAsync(
                stream,
                request.Photo.FileName,
                cancellationToken
            );
        }

        var chat = ChatFactory.CreateChat(new CreateChatOptions()
        {
            Type = ChatType.CommunityGroup,
            Name = request.Name,
            CommunityId = request.CommunityId,
            PhotoUrl = imageUrl
        });

        var userChat = new UserChat()
        {
            UserId = user.Value.Id,
            ChatId = chat.Value.Id
        };

        await userChatRepository.CreateAsync(userChat, cancellationToken);

        await chatRepository.CreateAsync(chat.Value, cancellationToken);

        logger.LogInformation("Chat created successfully with ID: {ChatId}", chat.Value.Id);

        var chatDto = ChatMapper.MapToChatGroupDTos(chat.Value);

        await chatSender.SendCreateChatAGroupAsync(chatDto);

        logger.LogInformation("Chat sent successfully with ID: {ChatId}", chatDto.ChatId);

        return ResultT<ChatGroupDTos>.Success(chatDto);
    }
}