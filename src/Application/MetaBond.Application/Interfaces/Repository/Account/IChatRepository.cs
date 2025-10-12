using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

/// <summary>
/// Defines a repository interface for managing and accessing chat entities in the application.
/// </summary>
/// <remarks>
/// Inherits functionality from the generic repository interface <c>IGenericRepository</c>,
/// using the <c>Chat</c> entity.
/// This repository is responsible for operations specifically related to the <c>Chat</c> entity,
/// such as retrieving, creating, updating, and deleting chat data.
/// </remarks>
public interface IChatRepository : IGenericRepository<Chat>
{
    /// <summary>
    /// Retrieves a paged list of chats for a specific user.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PagedResult<Chat>> GetPagedChatByUserIdAsync(int pageNumber, int pageSize, Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a chat by its ID, ensuring the specified user is a participant.
    /// </summary>
    /// <param name="chatId">The ID of the chat.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Chat?> GetByIdAndUserIdAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a user is a participant in a specific chat.
    /// </summary>
    /// <param name="chatId">The ID of the chat.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<bool> IsUserInChatAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all chats of a user, including the last message for each chat.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PagedResult<Chat>> GetChatsWithLastMessageAsync(Guid userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paged list of group chats associated with a specific community.
    /// </summary>
    /// <param name="communityId">The ID of the community.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PagedResult<Chat>> GetGroupChatsByCommunityIdAsync(Guid communityId, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the direct chat between two users, if it exists.
    /// </summary>
    /// <param name="userAId">The ID of the first user.</param>
    /// <param name="userBId">The ID of the second user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Chat?> GetDirectChatBetweenUsersAsync(Guid userAId, Guid userBId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all users participating in a specific chat.
    /// </summary>
    /// <param name="chatId">The ID of the chat.</param>
    /// <param name="pageSize"></param>
    /// <param name="pageNumber"></param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PagedResult<User>> GetUsersInChatAsync(Guid chatId, int pageSize, int pageNumber,
        CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a community group chat exists for a given community ID.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the community group chat exists.</returns>
    Task<bool> IsCommunityGroupChatExistAsync(Guid communityId, CancellationToken cancellationToken);


    /// <summary>
    /// Determines if a user is part of any chat.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>True if the user is part of any chat; otherwise, false.</returns>
    Task<bool> HasAnyChatAsync(Guid userId, CancellationToken cancellationToken);


    /// <summary>
    /// Checks if a chat has any participants.
    /// </summary>
    /// <param name="chatId">The unique identifier of the chat.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the chat has participants.</returns>
    Task<bool> HasParticipantsAsync(Guid chatId, CancellationToken cancellationToken);
}