using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Feature.Communities.Querys.Pagination
{
    internal sealed class GetPagedCommunitiesQueryHandler : IQueryHandler<GetPagedCommunitiesQuery, PagedResult<CommunitiesDTos>>
    {
        private readonly ICommunitiesRepository _communitiesRepository;

        public GetPagedCommunitiesQueryHandler(ICommunitiesRepository communitiesRepository)
        {
            _communitiesRepository = communitiesRepository;
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

                return ResultT<PagedResult<CommunitiesDTos>>.Success(pagedResult);
            }

            return ResultT<PagedResult<CommunitiesDTos>>.Failure(Error.Failure
                ("400", "No communities were found for the specified criteria."));
        }
    }
}
