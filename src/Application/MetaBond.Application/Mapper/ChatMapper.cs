using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.DTOs.Account.Message;
using MetaBond.Domain;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class ChatMapper
{
    public static ChatGroupDTos MapToChatGroupDTos(Chat chat)
    {
        return new ChatGroupDTos
        (
            ChatId: chat.Id,
            Name: chat.Name,
            Type: Enum.Parse<ChatType>(chat.Type!),
            Photo: chat.Photo
        );
    }

    public static ChatPrivateDTos MapToChatPrivateDTos(Chat chat)
    {
        return new ChatPrivateDTos(
            ChatId: chat.Id,
            Type: Enum.Parse<ChatType>(chat.Type!)
        );
    }

    public static ChatWithUserDTos MapToChatWithUserDTos(Chat chat, User user)
    {
        return new ChatWithUserDTos(
            ChatId: chat.Id,
            Type: Enum.Parse<ChatType>(chat.Type!),
            NameGroup: chat.Name,
            PhotoGroup: chat.Photo,
            User: new UserChatDTos(
                UserId: user.Id,
                Username: user.Username,
                Photo: user.Photo,
                FullName: $"{user.FirstName} {user.LastName}"
            )
        );
    }

    public static LeaveChatDTos MapToLeaveChatDTos(Chat chat, User user)
    {
        return new LeaveChatDTos(
            ChatId: chat.Id,
            Type: Enum.Parse<ChatType>(chat.Type!),
            UserId: user.Id
        );
    }

    public static ChatDTos MapToChatDTos(Chat chat)
    {
        return new ChatDTos(
            ChatId: chat.Id,
            Type: Enum.Parse<ChatType>(chat.Type!),
            Name: chat.Name,
            Photo: chat.Photo
        );
    }

    public static ChatGroupWithMessageDTos MapToChatGroupWithMessageDTos(Chat chat, IEnumerable<Message> messages)
    {
        return new ChatGroupWithMessageDTos(
            ChatId: chat.Id,
            Type: Enum.Parse<ChatType>(chat.Type!),
            Name: chat.Name,
            Photo: chat.Photo,
            CommunityId: chat.CommunityId,
            Messages: messages.Select(MessageMapper.MapToMessageWithChatDTos).ToList()
        );
    }

    public static ChatPrivateWithMessageDTos MapToChatPrivateWithMessageDTos(Chat chat, IEnumerable<Message> messages)
    {
        return new ChatPrivateWithMessageDTos(
            ChatId: chat.Id,
            Type: Enum.Parse<ChatType>(chat.Type!),
            Messages: messages.Select(MessageMapper.MapToMessageWithChatDTos).ToList()
        );
    }

    public static ChatWitLastMessageDTos MapToChatWitLastMessageDTos(Chat chat)
    {
        return new ChatWitLastMessageDTos(
            ChatId: chat.Id,
            Type: Enum.Parse<ChatType>(chat.Type!),
            LastMessage: chat.LastMessage!
        );
    }
}