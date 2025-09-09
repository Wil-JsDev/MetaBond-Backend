using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Query.GetCommunitiesByCategory;

internal sealed class GetCommunitiesByCategoryIdQueryHandler(
    ICommunitiesRepository communitiesRepository,
    IDistributedCache decoratedCache,
    ILogger<GetCommunitiesByCategoryIdQueryHandler> logger)
    : IQueryHandler<GetCommunitiesByCategoryIdQuery, PagedResult<CommunitiesByCategoryDto>>
{
    public async Task<ResultT<PagedResult<CommunitiesByCategoryDto>>> Handle(GetCommunitiesByCategoryIdQuery request,
        CancellationToken cancellationToken)
    {
        var validationPaginationResult = PaginationHelper.ValidatePagination<CommunitiesByCategoryDto>(
            request.PageNumber,
            request.PageSize,
            logger
        );

        if (!validationPaginationResult.IsSuccess)
            return validationPaginationResult;

        var communitiesCategoryDto = await decoratedCache.GetOrCreateAsync(
            $"get-paged-communities-{request.CategoryId}-{request.PageNumber}-{request.PageSize}",
            async () =>
            {
                var communities = await communitiesRepository.GetPagedCommunitiesByCategoryIdAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.CategoryId,
                    cancellationToken: cancellationToken);

                var dTos = communities.Items!.Select(CommunityMapper.MapCommunityByCategoryDto);

                PagedResult<CommunitiesByCategoryDto> pagedResult = new()
                {
                    TotalItems = communities.TotalItems,
                    CurrentPage = communities.CurrentPage,
                    TotalPages = communities.TotalPages,
                    Items = dTos
                };

                return pagedResult;
            },
            cancellationToken: cancellationToken
        );

        if (!communitiesCategoryDto.Items!.Any())
        {
            logger.LogError(
                "No communities found for CategoryId '{CategoryId}'. Request: {@Request}.",
                request.CategoryId,
                request
            );

            return ResultT<PagedResult<CommunitiesByCategoryDto>>.Failure(
                Error.Failure("400", $"No communities found for CategoryId '{request.CategoryId}'.")
            );
        }

        logger.LogInformation(
            "Found {Count} communities for CategoryId '{CategoryId}'. Request: {@Request}.",
            communitiesCategoryDto.Items!.Count(),
            request.CategoryId,
            request
        );

        return ResultT<PagedResult<CommunitiesByCategoryDto>>.Success(communitiesCategoryDto);
    }
}