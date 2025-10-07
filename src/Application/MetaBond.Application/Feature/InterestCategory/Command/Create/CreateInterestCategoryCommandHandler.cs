using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.InterestCategory;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.InterestCategory.Command.Create;

internal sealed class CreateInterestCategoryCommandHandler(
    ILogger<CreateInterestCategoryCommandHandler> logger,
    IInterestCategoryRepository interestCategoryRepository
) : ICommandHandler<CreateInterestCategoryCommand, InterestCategoryDTos>
{
    public async Task<ResultT<InterestCategoryDTos>> Handle(CreateInterestCategoryCommand request,
        CancellationToken cancellationToken)
    {
        if (await interestCategoryRepository.ExistsNameAsync(request.Name!, cancellationToken))
        {
            logger.LogWarning("The name {RequestName} already exists.", request.Name);

            return ResultT<InterestCategoryDTos>.Failure(
                Error.Failure("400", $"The name {request.Name} already exists."));
        }

        var interestCategory = new Domain.Models.InterestCategory()
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        await interestCategoryRepository.CreateAsync(interestCategory, cancellationToken);

        logger.LogInformation(
            "Interest Category with name '{InterestCategoryName}' was successfully created with ID {InterestCategoryId}.",
            interestCategory.Name, interestCategory.Id);

        return ResultT<InterestCategoryDTos>.Success(InterestCategoryMapper.MapInterestCategoryDTos(interestCategory));
    }
}