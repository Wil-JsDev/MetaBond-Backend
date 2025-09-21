using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetProgressEntriesWithAuthor;

public class GetProgressEntriesWithAuthorsQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    ILogger<GetProgressEntriesWithAuthorsQueryHandler> logger,
    IDistributedCache decorated) :
    IQueryHandler<GetProgressEntriesWithAuthorsQuery,
        PagedResult<ProgressEntriesWithUserDTos>>
{
    public async Task<ResultT<PagedResult<ProgressEntriesWithUserDTos>>> Handle(
        GetProgressEntriesWithAuthorsQuery request, CancellationToken cancellationToken)
    {
        var progressEntry = await EntityHelper.GetEntityByIdAsync
        (
            progressEntryRepository.GetByIdAsync,
            request.ProgressEntryId,
            "ProgressEntry",
            logger
        );
        if (!progressEntry.IsSuccess)
            return progressEntry.Error!;

        var validationPagination =
            PaginationHelper.ValidatePagination<ProgressEntriesWithUserDTos>(request.PageNumber,
                request.PageSize,
                logger);

        if (!validationPagination.IsSuccess)
            return validationPagination.Error;

        var result = await decorated.GetOrCreateAsync(
            $"Get-Progress-Entries-With-Authors-{request.ProgressEntryId}",
            async () =>
            {
                var progressEntries = await progressEntryRepository.GetProgressEntriesWithAuthorsAsync(
                    request.ProgressEntryId,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                var items = progressEntries.Items ?? [];

                PagedResult<ProgressEntriesWithUserDTos> pagedResult = new()
                {
                    CurrentPage = progressEntries.CurrentPage,
                    Items = items.ProgressEntriesWithUserToListDto(),
                    TotalItems = progressEntries.TotalItems,
                    TotalPages = progressEntries.TotalPages
                };

                return pagedResult;
            },
            cancellationToken: cancellationToken);

        var progressEntriesWithUserDTosEnumerable = result.Items ?? [];
        if (!progressEntriesWithUserDTosEnumerable.Any())
        {
            logger.LogWarning("No progress entries found with authors for the given ID.");

            return ResultT<PagedResult<ProgressEntriesWithUserDTos>>.Failure(Error.NotFound("404",
                "No progress entries found."));
        }

        logger.LogInformation("Progress entries with author information retrieved successfully.");

        return ResultT<PagedResult<ProgressEntriesWithUserDTos>>.Success(result);
    }
}