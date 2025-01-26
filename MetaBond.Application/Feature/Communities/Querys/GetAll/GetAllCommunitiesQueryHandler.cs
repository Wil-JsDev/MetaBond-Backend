using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Communities.Querys.GetAll
{
    internal sealed class GetAllCommunitiesQueryHandler : IQueryHandler<GetlAllCommunitiesQuery,IEnumerable<CommunitiesDTos>>
    {
        private readonly ICommunitiesRepository _communitiesRepository;

        public GetAllCommunitiesQueryHandler(ICommunitiesRepository communitiesRepository)
        {
            _communitiesRepository = communitiesRepository;
        }

        public async Task<ResultT<IEnumerable<CommunitiesDTos>>> Handle(GetlAllCommunitiesQuery request, CancellationToken cancellationToken)
        {
            var communitiesAll = await _communitiesRepository.GetAll(cancellationToken);
            if (communitiesAll != null)
            {
                var communitiesDtosList = communitiesAll.Select(c => new CommunitiesDTos
                (
                    CommunitieId: c.Id,
                    Name: c.Name,
                    Category: c.Category,
                    CreatedAt: c.CreateAt
                ));

                return ResultT<IEnumerable<CommunitiesDTos>>.Success(communitiesDtosList);
            }

            return ResultT<IEnumerable<CommunitiesDTos>>.Failure(Error.Failure("400", "List of null"));
        }
    }
}
