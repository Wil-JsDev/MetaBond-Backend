using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.DTOs.Account.Message;

namespace MetaBond.Application.Interfaces.Service.SignaIR.Senders;

/// <summary>
/// Defines the server-to-client contract for broadcasting chat-related events.
/// This interface is typically used by a SignalR Hub to invoke methods on connected clients.
/// </summary>
public interface IChatSender
{
    /// <summary>
    /// Notifies clients that a new group chat has been created.
    /// </summary>
    /// <param name="chatGroup">The data transfer object containing the details of the new group chat.</param>
    Task SendCreateChatAGroupAsync(ChatGroupDTos chatGroup);

    /// <summary>
    /// Notifies the relevant clients that a new private chat has been created.
    /// </summary>
    /// <param name="chatPrivate">The data transfer object containing the details of the new private chat.</param>
    Task SendCreateChatPrivateAsync(ChatPrivateDTos chatPrivate);

    /// <summary>
    /// Notifies clients within a specific chat that a new user has joined.
    /// </summary>
    /// <param name="chatId">The ID of the chat that the user joined.</param>
    /// <param name="chatWithUser">The data transfer object identifying the user who joined.</param>
    Task SendJoinChatAsync(Guid chatId, ChatWithUserDTos chatWithUser);

    /// <summary>
    /// Notifies clients within a specific chat that a user has left.
    /// </summary>
    /// <param name="chatId">The ID of the chat that the user left.</param>
    /// <param name="chatWithUser">The data transfer object identifying the user who left.</param>
    Task SendLeaveChatAsync(Guid chatId, ChatWithUserDTos chatWithUser);

    /// <summary>
    /// Broadcasts a new message to all clients in a specific chat.
    /// </summary>
    /// <param name="chatId">The ID of the chat where the message was sent.</param>
    /// <param name="message">The message data transfer object, containing content and metadata.</param>
    Task SendMessageAsync(Guid chatId, MessageDto message);

    Task SendMessagesReadAsync(Guid chatId, Guid userId, IReadOnlyCollection<Guid> messageIds);
}