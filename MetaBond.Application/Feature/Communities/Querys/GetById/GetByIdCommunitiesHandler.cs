using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Communities.Querys.GetById
{
    public class GetByIdCommunitiesHandler : IQueryHandler<GetByIdCommunitiesQuery, CommunitiesDTos>
    {

        private readonly ICommunitiesRepository _communitiesRepository;

        public GetByIdCommunitiesHandler(ICommunitiesRepository communitiesRepository)
        {
            _communitiesRepository = communitiesRepository;
        }

        public async Task<ResultT<CommunitiesDTos>> Handle(GetByIdCommunitiesQuery request, CancellationToken cancellationToken)
        {
            var communities = await _communitiesRepository.GetByIdAsync(request.Id);
            if (communities != null)
            {
                CommunitiesDTos dTos = new
                (
                    CommunitieId: communities.Id,
                    Name: communities.Name,
                    Category: communities.Category,
                    CreatedAt: communities.CreateAt
                );
                
                return ResultT<CommunitiesDTos>.Success(dTos);
            }

            return ResultT<CommunitiesDTos>.Failure(Error.NotFound("404", $"Community with ID {request.Id} was not found."));
        }
    }
}
