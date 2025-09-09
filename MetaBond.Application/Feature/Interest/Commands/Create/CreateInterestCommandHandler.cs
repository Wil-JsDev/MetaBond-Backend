using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Interest;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Interest.Commands.Create;

internal sealed class CreateInterestCommandHandler(
    ILogger<CreateInterestCommandHandler> logger,
    IInterestCategoryRepository interestCategoryRepository,
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

        var interestCategory = await EntityHelper.GetEntityByIdAsync(
            interestCategoryRepository.GetByIdAsync,
            request.InterestCategoryId ?? Guid.Empty,
            "Interest Category",
            logger
        );

        if (!interestCategory.IsSuccess) return ResultT<InterestDTos>.Failure(interestCategory.Error!);

        Domain.Models.Interest interest = new()
        {
            Id = Guid.NewGuid(),
            Name = request.Name!,
            InterestCategoryId = request.InterestCategoryId
        };

        await interestRepository.CreateAsync(interest, cancellationToken);

        logger.LogInformation("Interest with name '{InterestName}' was successfully created with ID {InterestId}.",
            interest.Name, interest.Id);

        var interestDto = InterestMapper.ModelToDto(interest);
        return ResultT<InterestDTos>.Success(interestDto);
    }
}