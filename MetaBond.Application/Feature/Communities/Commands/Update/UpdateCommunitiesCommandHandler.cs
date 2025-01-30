using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Feature.Communities.Commands.Update
{
    internal sealed class UpdateCommunitiesCommandHandler : ICommandHandler<UpdateCommunitiesCommand, CommunitiesDTos>
    {
        private readonly ICommunitiesRepository _communitiesRepository;

        public UpdateCommunitiesCommandHandler(ICommunitiesRepository communitiesRepository)
        {
            _communitiesRepository = communitiesRepository;
        }

        public async Task<ResultT<CommunitiesDTos>> Handle(UpdateCommunitiesCommand request, CancellationToken cancellationToken)
        {
            var communites = await _communitiesRepository.GetByIdAsync(request.Id);

            if (communites != null)
            {
                communites.Name = request.Name;
                communites.Category = request.Category;

                await _communitiesRepository.UpdateAsync(communites, cancellationToken);

                CommunitiesDTos communitiesDTos = new(
                    CommunitieId: communites.Id,
                    Name: communites.Name,
                    Category: communites.Category,
                    CreatedAt: communites.CreateAt
                );

                return ResultT<CommunitiesDTos>.Success(communitiesDTos);
            }

            return ResultT<CommunitiesDTos>.Failure(Error.Failure("404", $"{request.Id} not found"));
        }
    }
}
