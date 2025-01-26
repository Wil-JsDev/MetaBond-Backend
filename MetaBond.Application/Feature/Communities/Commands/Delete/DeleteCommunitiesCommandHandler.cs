using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Communities.Commands.Delete
{
    internal sealed class DeleteCommunitiesCommandHandler : ICommandHandler<DeleteCommunitiesCommand, Guid>
    {

        private readonly ICommunitiesRepository _communitiesRepository;
        public DeleteCommunitiesCommandHandler(ICommunitiesRepository communitiesRepository)
        {
            _communitiesRepository = communitiesRepository;
        }

        public async Task<ResultT<Guid>> Handle(DeleteCommunitiesCommand request, CancellationToken cancellationToken)
        {
            var communities = await _communitiesRepository.GetByIdAsync(request.Id);
            if (communities != null)
            {
                await _communitiesRepository.DeleteAsync(communities,cancellationToken);
                return ResultT<Guid>.Success(request.Id);
            }

            return ResultT<Guid>.Failure(Error.NotFound("404", $"{request.Id} not found"));

        }
    }
}
