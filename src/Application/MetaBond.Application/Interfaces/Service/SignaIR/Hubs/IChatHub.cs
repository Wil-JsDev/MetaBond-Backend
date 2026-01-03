using MetaBond.Application.DTOs.Account.Chat;

namespace MetaBond.Application.Interfaces.Service.SignaIR.Hubs;

/// <summary>
/// Defines the client-to-server contract for a real-time chat hub.
/// </summary>
public interface IChatHub
{
    /// <summary>
    /// Notifies the server when a client creates a new group chat.
    /// </summary>
    /// <param name="chatGroup">The data transfer object containing the details of the new group chat.</param>
    Task OnCreatedGroupChat(ChatGroupDTos chatGroup);

    /// <summary>
    /// Notifies the server when a client creates a new private chat.
    /// </summary>
    /// <param name="chatPrivate">The data transfer object containing the details of the new private chat.</param>
    Task OnCreatedChatPrivate(ChatPrivateDTos chatPrivate);

    /// <summary>
    /// Allows a client to join an existing chat.
    /// </summary>
    /// <param name="chatWithUser">The data transfer object containing the chat and user identifiers.</param>
    Task OnJoinChat(ChatWithUserDTos chatWithUser);

    /// <summary>
    /// Allows a client to leave a chat they are a part of.
    /// </summary>
    /// <param name="chatWithUser">The data transfer object containing the chat and user identifiers.</param>
    Task OnLeaveChat(ChatWithUserDTos chatWithUser);

    /// <summary>
    /// Handles a new message sent by a client to be broadcasted to a specific chat.
    /// </summary>
    /// <param name="chatId">The ID of the target chat room.</param>
    /// <param name="userId">The ID of the user sending the message.</param>
    /// <param name="message">The content of the message.</param>
    Task OnMessageReceived(Guid chatId, Guid userId, string message);

    Task OnMessagesRead(Guid chatId, Guid userId, IReadOnlyCollection<Guid> messageIds);
}