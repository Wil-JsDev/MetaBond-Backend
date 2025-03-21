﻿using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Commands
{
    internal sealed class CreateCommuntiesCommandHandler : 
        ICommandHandler<CreateCommuntiesCommand, CommunitiesDTos>
    {
        private readonly ICommunitiesRepository _communitiesRepository;
        private readonly ILogger<CreateCommuntiesCommandHandler> _logger;
        public CreateCommuntiesCommandHandler(ICommunitiesRepository communitiesRepository, ILogger<CreateCommuntiesCommandHandler> logger)
        {
            _communitiesRepository = communitiesRepository;
            _logger = logger;
        }

        public async Task<ResultT<CommunitiesDTos>> Handle(
            CreateCommuntiesCommand request, 
            CancellationToken cancellationToken)
        {

            if (request != null)
            {
                var exists = await _communitiesRepository.ValidateAsync(x => x.Name == request.Name);
                if (exists)
                {
                    _logger.LogError($"The name {request.Name} already exists.");

                    return ResultT<CommunitiesDTos>.Failure(Error.Failure("400", $"The name {request.Name} already exists."));
                }
                
                Domain.Models.Communities communities = new()
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    Category = request.Category
                };

                await _communitiesRepository.CreateAsync(communities,cancellationToken);
                _logger.LogInformation("Community {CommunityId} created successfully.", communities.Id);

                CommunitiesDTos communitiesDTos = new(
                    CommunitieId: communities.Id,
                    Name: communities.Name,
                    Category: communities.Category,
                    CreatedAt: communities.CreateAt
                );
                
                return ResultT<CommunitiesDTos>.Success(communitiesDTos);
            }
            _logger.LogError("Received a null CreateCommunitiesCommand request.");
            return ResultT<CommunitiesDTos>.Failure(Error.Failure("400", "The request object is null"));
        }
    }
}
