using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
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
        public async Task<ResultT<PagedResult<CommunitiesDTos>>> Handle(GetPagedCommunitiesQuery request, CancellationToken cancellationToken)
        {
            if (request != null)
            {
                var cacheKey = $"paged-communities-page-{request.PageNumber}-size-{request.PageSize}";
                var communitiesPaged = await decoratedCache.GetOrCreateAsync(cacheKey,
                    async () => await communitiesRepository.GetPagedCommunitiesAsync(request.PageNumber,
                        request.PageSize, cancellationToken), cancellationToken: cancellationToken);                
                
                var dtoItems = communitiesPaged.Items!.Select(c => new CommunitiesDTos(
                    CommunitieId: c.Id,
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

                logger.LogInformation("Successfully retrieved {TotalItems} communities (Page {CurrentPage} of {TotalPages}).",
                                      pagedResult.TotalItems, pagedResult.CurrentPage, pagedResult.TotalPages);

                return ResultT<PagedResult<CommunitiesDTos>>.Success(pagedResult);

            }

            logger.LogError("Failed to retrieve paged communities. The request object is null.");

            return ResultT<PagedResult<CommunitiesDTos>>.Failure(Error.Failure
                ("400", "No communities were found for the specified criteria."));

        }
    }
