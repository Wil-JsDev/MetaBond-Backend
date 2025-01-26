using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Feature.Communities.Querys.Filter
{
    internal sealed class FilterCommunitiesQueryHandler : IQueryHandler<FilterCommunitiesQuery,IEnumerable<CommunitiesDTos>>
    {
        private readonly ICommunitiesRepository _communitiesRepository;

        public FilterCommunitiesQueryHandler(ICommunitiesRepository communitiesRepository)
        {
            _communitiesRepository = communitiesRepository;
        }

        public async Task<ResultT<IEnumerable<CommunitiesDTos>>> Handle(FilterCommunitiesQuery request, CancellationToken cancellationToken)
        {
            
            if (request.Category != null)
            {
                var communitiesByCatagory = await _communitiesRepository.GetByFilterAsync(x => x.Category == request.Category,cancellationToken);

                var dTos = communitiesByCatagory.Select(c => new CommunitiesDTos
                (
                   CommunitieId: c.Id,
                   Name: c.Name,
                   Category: c.Category,
                   CreatedAt: c.CreateAt
                ));

                return ResultT<IEnumerable<CommunitiesDTos>>.Success(dTos);

            }
            return ResultT<IEnumerable<CommunitiesDTos>>.Failure(Error.Failure("404", $"{request.Category} not found"));
        }
    }
}
