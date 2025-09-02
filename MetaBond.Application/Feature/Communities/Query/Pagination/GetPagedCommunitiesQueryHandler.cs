using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Query.Pagination;

internal sealed class GetPagedCommunitiesQueryHandler(
    ICommunitiesRepository communitiesRepository,
    IDistributedCache decoratedCache,
    ILogger<GetPagedCommunitiesQueryHandler> logger)
    : IQueryHandler<GetPagedCommunitiesQuery, PagedResult<CommunitiesDTos>>
{
    public async Task<ResultT<PagedResult<CommunitiesDTos>>> Handle(GetPagedCommunitiesQuery request,
        CancellationToken cancellationToken)
    {
        var validationPagination =
            PaginationHelper.ValidatePagination<CommunitiesDTos>(request.PageNumber, request.PageSize, logger);

        if (!validationPagination.IsSuccess)
            return validationPagination;

        var cacheKey = $"paged-communities-page-{request.PageNumber}-size-{request.PageSize}";
        var result = await decoratedCache.GetOrCreateAsync(cacheKey,
            async () =>
            {
                var communitiesPaged = await communitiesRepository.GetPagedCommunitiesAsync(request.PageNumber,
                    request.PageSize, cancellationToken);

                var dtoItems = communitiesPaged.Items!.Select(c => new CommunitiesDTos(
                    CommunitiesId: c.Id,
                    Name: c.Name,
                    Category: c.Category,
                    CreatedAt: c.CreateAt
                )).ToList();

                PagedResult<CommunitiesDTos> pagedResult = new()
                {
                    TotalItems = communitiesPaged.TotalItems,
                    CurrentPage = communitiesPaged.CurrentPage,
                    TotalPages = communitiesPaged.TotalPages,
                    Items = dtoItems
                };

                return pagedResult;
            }, cancellationToken: cancellationToken);

        logger.LogInformation("Successfully retrieved {TotalItems} communities (Page {CurrentPage} of {TotalPages}).",
            result.TotalItems, result.CurrentPage, result.TotalPages);

        return ResultT<PagedResult<CommunitiesDTos>>.Success(result);
    }
}