using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

/// <summary>
/// Represents a repository interface for managing user-chat relationships.
/// Provides methods for accessing and manipulating user-chat data.
/// </summary>
public interface IUserChatRepository : IGenericRepository<UserChat>
{
    /// <summary>
    /// Retrieves the UserChat relationship for a specific user and chat.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="chatId">The ID of the chat.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<UserChat?> GetByUserIdAndChatIdAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all UserChat entries for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PagedResult<UserChat>> GetChatsByUserIdAsync(Guid userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all UserChat entries for a specific chat.
    /// </summary>
    /// <param name="chatId">The ID of the chat.</param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PagedResult<UserChat>> GetUsersByChatIdAsync(Guid chatId, int pageNumber, int pageSize,
        CancellationToken cancellationToken);


    /// <summary>
    /// Removes a user from a chat by deleting the UserChat relationship.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="chatId">The ID of the chat.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveUserFromChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new UserChat relationship to the data store.
    /// </summary>
    /// <param name="userChat">The UserChat object representing the relationship between a user and a chat.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddUserToChatAsync(UserChat userChat, CancellationToken cancellationToken);
}