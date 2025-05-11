using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetProgressEntriesWithAuthor;

public class GetProgressEntriesWithAuthorsQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    ILogger<GetProgressEntriesWithAuthorsQueryHandler> logger,
    IDistributedCache decorated): 
    IQueryHandler<GetProgressEntriesWithAuthorsQuery, 
        IEnumerable<ProgressEntriesWithUserDTos>>
{
    public async Task<ResultT<IEnumerable<ProgressEntriesWithUserDTos>>> Handle(GetProgressEntriesWithAuthorsQuery request, CancellationToken cancellationToken)
    {
        if (request != null)
        {
            var progressEntryId = await progressEntryRepository.GetByIdAsync(request.ProgressEntryId);
            if (progressEntryId == null)
            {
                logger.LogError($"ProgressEntry with id {request.ProgressEntryId} not found");
        
                return ResultT<IEnumerable<ProgressEntriesWithUserDTos>>.Failure(Error.NotFound("404", "Progress entry not found"));
            }
    
            var progressEntry = await decorated.GetOrCreateAsync($"Get-Progress-Entries-With-Authors-{request.ProgressEntryId}", 
                async () => await progressEntryRepository.GetProgressEntriesWithAuthorsAsync(request.ProgressEntryId, 
                    cancellationToken), 
                cancellationToken: cancellationToken);

            IEnumerable<Domain.Models.ProgressEntry> progressEntries = progressEntry.ToList();
            if (!progressEntries.Any())
            {
                logger.LogWarning("No progress entries found with authors for the given ID.");
        
                return ResultT<IEnumerable<ProgressEntriesWithUserDTos>>.Failure(Error.NotFound("404","No progress entries found."));
            }

            var progressEntriesWithUserDTos = progressEntries.Select(x =>
                new ProgressEntriesWithUserDTos
                (
                    ProgressEntryId: x.Id,
                    ProgressBoardId: x.ProgressBoardId,
                    User: new UserProgressEntryDTos(
                        UserId: x.User!.Id,
                        Username: x.User.Username,
                        Photo: x.User.Photo
                    ),
                    Description: x.Description,
                    CreatedAt: x.CreatedAt,
                    UpdateAt: x.UpdateAt
                ));
    
            logger.LogInformation("Progress entries with author information retrieved successfully.");

            return ResultT<IEnumerable<ProgressEntriesWithUserDTos>>.Success(progressEntriesWithUserDTos);
        }
        logger.LogWarning("Request object is null. Cannot proceed with retrieving progress entries.");

        return ResultT<IEnumerable<ProgressEntriesWithUserDTos>>.Failure(Error.NotFound("404", "Progress entry not found"));
    }
}