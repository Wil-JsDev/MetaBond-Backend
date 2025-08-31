using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

public interface IUserInterestRepository : IGenericRepository<UserInterest>
{
    /// <summary>
    /// Associates a collection of interests to a user.
    /// </summary>
    /// <remarks>
    /// Only adds new interests that the user does not already have. 
    /// Existing associations are not removed or updated.
    /// </remarks>
    /// <param name="userId">The ID of the user to associate interests with.</param>
    /// <param name="interestIds">The list of interest IDs to associate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A read-only collection of <see cref="UserInterest"/> representing the newly created associations.
    /// </returns>
    Task<IReadOnlyCollection<UserInterest>> AssociateInterestsToUserAsync(
        Guid userId,
        IEnumerable<Guid> interestIds,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds multiple UserInterest entities to the database.
    /// </summary>
    /// <param name="userInterests">A list of UserInterest entities to create.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    Task CreateMultipleUserInterestAsync(
        List<UserInterest> userInterests,
        CancellationToken cancellationToken);
}