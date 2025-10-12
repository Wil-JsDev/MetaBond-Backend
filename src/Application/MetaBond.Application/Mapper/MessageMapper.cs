using MetaBond.Application.DTOs.Account.Message;
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
}