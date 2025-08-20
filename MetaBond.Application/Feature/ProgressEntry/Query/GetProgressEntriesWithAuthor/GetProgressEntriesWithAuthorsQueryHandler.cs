using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetProgressEntriesWithAuthor;

public class GetProgressEntriesWithAuthorsQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    ILogger<GetProgressEntriesWithAuthorsQueryHandler> logger,
    IDistributedCache decorated) :
    IQueryHandler<GetProgressEntriesWithAuthorsQuery,
        IEnumerable<ProgressEntriesWithUserDTos>>
{
    public async Task<ResultT<IEnumerable<ProgressEntriesWithUserDTos>>> Handle(
        GetProgressEntriesWithAuthorsQuery request, CancellationToken cancellationToken)
    {
        var progressEntryId = await progressEntryRepository.GetByIdAsync(request.ProgressEntryId);
        if (progressEntryId == null)
        {
            logger.LogError($"ProgressEntry with id {request.ProgressEntryId} not found");

            return ResultT<IEnumerable<ProgressEntriesWithUserDTos>>.Failure(Error.NotFound("404",
                "Progress entry not found"));
        }

        var result = await decorated.GetOrCreateAsync(
            $"Get-Progress-Entries-With-Authors-{request.ProgressEntryId}",
            async () =>
            {
                var progressEntries = await progressEntryRepository.GetProgressEntriesWithAuthorsAsync(
                    request.ProgressEntryId,
                    cancellationToken);

                return progressEntries.ProgressEntriesWithUserToListDto();
            },
            cancellationToken: cancellationToken);

        var progressEntriesWithUserDTosEnumerable = result.ToList();
        if (!progressEntriesWithUserDTosEnumerable.Any())
        {
            logger.LogWarning("No progress entries found with authors for the given ID.");

            return ResultT<IEnumerable<ProgressEntriesWithUserDTos>>.Failure(Error.NotFound("404",
                "No progress entries found."));
        }

        logger.LogInformation("Progress entries with author information retrieved successfully.");

        return ResultT<IEnumerable<ProgressEntriesWithUserDTos>>.Success(progressEntriesWithUserDTosEnumerable);
    }
}