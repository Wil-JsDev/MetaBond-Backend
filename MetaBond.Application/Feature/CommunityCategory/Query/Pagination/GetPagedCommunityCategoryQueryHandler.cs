using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.CommunityCategory;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.CommunityCategory.Query.Pagination;

internal sealed class GetPagedCommunityCategoryQueryHandler(
    ILogger<GetPagedCommunityCategoryQueryHandler> logger,
    ICommunityCategoryRepository communityCategoryRepository,
    IDistributedCache cache
) : IQueryHandler<GetPagedCommunityCategoryQuery, PagedResult<CommunityCategoryDTos>>
{
    public async Task<ResultT<PagedResult<CommunityCategoryDTos>>> Handle(GetPagedCommunityCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var validationPaginationResult = PaginationHelper.ValidatePagination<CommunityCategoryDTos>
        (
            request.PageNumber,
            request.PageSize,
            logger
        );

        if (!validationPaginationResult.IsSuccess)
            return validationPaginationResult;

        var result = await cache.GetOrCreateAsync(
            $"get-paged-community-category-{request.PageNumber}-{request.PageSize}",
            async () =>
            {
                var paged = await communityCategoryRepository.GetPagedAsync(request.PageNumber, request.PageSize,
                    cancellationToken);

                var pagedDto = paged.Items!.Select(CommunityCategoryMapper.MapCommunityCategoryDTos).ToList();

                PagedResult<CommunityCategoryDTos> pagedResult = new()
                {
                    TotalItems = paged.TotalItems,
                    CurrentPage = paged.CurrentPage,
                    TotalPages = paged.TotalPages,
                    Items = pagedDto
                };

                return pagedResult;
            }, cancellationToken: cancellationToken);

        if (result.Items is null || !result.Items.Any())
        {
            logger.LogWarning(
                "GetPagedCommunityCategoryQueryHandler: No Community Categories found for the given query.");

            return ResultT<PagedResult<CommunityCategoryDTos>>.Failure(
                Error.NotFound("404", "No Community Categories found."));
        }

        logger.LogInformation(
            "GetPagedCommunityCategoryQueryHandler: {Count} Community Categories retrieved successfully.",
            result.Items.Count());

        return ResultT<PagedResult<CommunityCategoryDTos>>.Success(result);
    }
}