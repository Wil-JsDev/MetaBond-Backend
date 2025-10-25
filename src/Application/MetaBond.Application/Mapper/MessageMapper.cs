using MetaBond.Application.DTOs.Account.Message;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class MessageMapper
{
    public static MessageWithChatDTos MapToMessageWithChatDTos(Message message)
    {
        return new MessageWithChatDTos(
            message.Id,
            message.Content ?? string.Empty,
            message.SentAt,
            UserMapper.MapToSenderDTos(message.Sender ?? new User())
        );
    }

    public static MessageDto MapToMessageDto(Message message)
    {
        return new MessageDto(
            MessageId: message.Id,
            Content: message.Content ?? string.Empty,
            UserId: message.SenderId,
            SentAt: message.SentAt
        );
    }

    public static MessageWithUserDTos MessageWithUserDTos(Message message, User user)
    {
        return new MessageWithUserDTos(
            MessageId: message.Id,
            Content: message.Content ?? string.Empty,
            SentAt: message.SentAt,
            Sender: new SenderDTos(
                SenderId: user.Id,
                DisplayName: user.Username,
                Photo: user.Photo
            )
        );
    }

    public static MessageGeneralDTos MapToMessageGeneralDTos(Message message, User user)
    {
        return new MessageGeneralDTos(
            MessageId: message.Id,
            ChatId: message.ChatId,
            Content: message.Content ?? string.Empty,
            SentAt: message.SentAt,
            IsEdited: message.IsEdited,
            EditedAt: message.EditedAt,
            IsDeleted: message.IsDeleted,
            Sender: new SenderDTos(
                SenderId: user.Id,
                DisplayName: user.Username,
                Photo: user.Photo
            )
        );
    }

    public static UpdateEditedMessageDTos MapToUpdateEditedMessageDTos(Message message)
    {
        return new UpdateEditedMessageDTos(
            Content: message.Content ?? string.Empty
        );
    }
}