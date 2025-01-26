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
                Domain.Models.Communities communitiesModel = new()
                {
                    Name = communites.Name,
                    Category = communites.Category
                };
                await _communitiesRepository.UpdateAsync(communitiesModel, cancellationToken);

                CommunitiesDTos communitiesDTos = new(
                    CommunitieId: communitiesModel.Id,
                    Name: communitiesModel.Name,
                    Category: communitiesModel.Category,
                    CreatedAt: communitiesModel.CreateAt
                );

                return ResultT<CommunitiesDTos>.Success(communitiesDTos);

            }

            return ResultT<CommunitiesDTos>.Failure(Error.Failure("404", ""));
        }
    }
}
