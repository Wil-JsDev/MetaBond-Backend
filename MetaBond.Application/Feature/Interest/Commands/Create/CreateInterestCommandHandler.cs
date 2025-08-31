using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Interest;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Interest.Commands.Create;

internal sealed class CreateInterestCommandHandler(
    ILogger<CreateInterestCommandHandler> logger,
    IInterestRepository interestRepository
) : ICommandHandler<CreateInterestCommand, InterestDTos>
{
    public async Task<ResultT<InterestDTos>> Handle(CreateInterestCommand request, CancellationToken cancellationToken)
    {
        // Check if interest already exists
        if (await interestRepository.InterestExistsAsync(request.Name!, cancellationToken))
        {
            logger.LogWarning("Interest with name '{InterestName}' already exists.", request.Name!);

            return ResultT<InterestDTos>.Failure(
                Error.Conflict("400", $"Interest with name '{request.Name!}' already exists."));
        }

        Domain.Models.Interest interest = new()
        {
            Id = Guid.NewGuid(),
            Name = request.Name!
        };

        await interestRepository.CreateAsync(interest, cancellationToken);

        logger.LogInformation("Interest with name '{InterestName}' was successfully created with ID {InterestId}.",
            interest.Name, interest.Id);

        var interestDto = InterestMapper.ModelToDto(interest);
        return ResultT<InterestDTos>.Success(interestDto);
    }
}