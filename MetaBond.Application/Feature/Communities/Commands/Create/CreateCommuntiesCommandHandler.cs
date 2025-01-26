using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Feature.Communities.Commands
{
    internal sealed class CreateCommuntiesCommandHandler : 
        ICommandHandler<CreateCommuntiesCommand, CommunitiesDTos>
    {
        private readonly ICommunitiesRepository _communitiesRepository;
        public CreateCommuntiesCommandHandler(ICommunitiesRepository communitiesRepository)
        {
            _communitiesRepository = communitiesRepository;
        }

        public async Task<ResultT<CommunitiesDTos>> Handle(
            CreateCommuntiesCommand request, 
            CancellationToken cancellationToken)
        {

            if (request != null)
            { 
                Domain.Models.Communities communities = new()
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Category = request.Category
                };

                await _communitiesRepository.CreateAsync(communities,cancellationToken);

                CommunitiesDTos communitiesDTos = new(
                    CommunitieId: communities.Id,
                    Name: communities.Name,
                    Category: communities.Category,
                    CreatedAt: communities.CreateAt
                );

                return ResultT<CommunitiesDTos>.Success(communitiesDTos);
            }

            return ResultT<CommunitiesDTos>.Failure(Error.Failure("400", "The request object is null"));
        }
    }
}
