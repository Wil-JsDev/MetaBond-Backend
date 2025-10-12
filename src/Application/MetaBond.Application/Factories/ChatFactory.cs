using MetaBond.Application.Factories.Parameters;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Factories;

public static class ChatFactory
{
    public static ResultT<Chat> CreateChat(CreateChatOptions chatOptions)
    {
        if (!Enum.IsDefined(typeof(ChatType), chatOptions.Type))
        {
            return ResultT<Chat>.Failure(Error.Failure("400", "Chat type is not valid."));
        }

        return chatOptions.Type switch
        {
            ChatType.CommunityGroup => CreateCommunityGroupChat(chatOptions),
            ChatType.Direct => CreateDirectChat(chatOptions),
            _ => ResultT<Chat>.Failure(Error.Failure("400", "Chat type is not supported.")),
        };
    }

    private static ResultT<Chat> CreateCommunityGroupChat(CreateChatOptions options)
    {
        var chat = new Chat()
        {
            Id = Guid.NewGuid(),
            Name = options.Name,
            Type = options.Type.ToString(),
            CommunityId = options.CommunityId,
            Photo = options.PhotoUrl
        };

        return ResultT<Chat>.Success(chat);
    }

    private static ResultT<Chat> CreateDirectChat(CreateChatOptions options)
    {
        var chat = new Chat()
        {
            Id = Guid.NewGuid(),
            Name = null,
            Type = options.Type.ToString(),
            CommunityId = null,
            Photo = null
        };

        return ResultT<Chat>.Success(chat);
    }
}