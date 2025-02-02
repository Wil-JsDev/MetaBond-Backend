using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Querys.Pagination
{
    internal sealed class GetPagedCommunitiesQueryHandler : IQueryHandler<GetPagedCommunitiesQuery, PagedResult<CommunitiesDTos>>
    {
        private readonly ICommunitiesRepository _communitiesRepository;
        private readonly ILogger<GetPagedCommunitiesQueryHandler> _logger;

        public GetPagedCommunitiesQueryHandler(ICommunitiesRepository communitiesRepository, ILogger<GetPagedCommunitiesQueryHandler> logger)
        {
            _communitiesRepository = communitiesRepository;
            _logger = logger;
        }

        public async Task<ResultT<PagedResult<CommunitiesDTos>>> Handle(GetPagedCommunitiesQuery request, CancellationToken cancellationToken)
        {
            if (request != null)
            {
                var communitiesPagedWithNumbe = await _communitiesRepository.GetPagedCommunitiesAsync(request.PageNumber, request.PageSize,cancellationToken);
                var dtoItems = communitiesPagedWithNumbe.Items.Select(c => new CommunitiesDTos(
                    CommunitieId: c.Id,
                    Name: c.Name,
                    Category: c.Category,
                    CreatedAt: c.CreateAt
                )).ToList();

                PagedResult<CommunitiesDTos> pagedResult = new()
                {
                    TotalItems = communitiesPagedWithNumbe.TotalItems,
                    CurrentPage = communitiesPagedWithNumbe.CurrentPage,
                    TotalPages = communitiesPagedWithNumbe.TotalPages,
                    Items = dtoItems
                };

                _logger.LogInformation("Successfully retrieved {TotalItems} communities (Page {CurrentPage} of {TotalPages}).",
                                      pagedResult.TotalItems, pagedResult.CurrentPage, pagedResult.TotalPages);

                return ResultT<PagedResult<CommunitiesDTos>>.Success(pagedResult);

            }

            _logger.LogError("Failed to retrieve paged communities. The request object is null.");

            return ResultT<PagedResult<CommunitiesDTos>>.Failure(Error.Failure
                ("400", "No communities were found for the specified criteria."));

        }
    }
}
