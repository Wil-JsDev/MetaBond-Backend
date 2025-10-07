using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.InterestCategory;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.InterestCategory.Command.Update;

internal sealed class UpdateInterestCategoryCommandHandler(
    ILogger<UpdateInterestCategoryCommandHandler> logger,
    IInterestCategoryRepository interestCategoryRepository
) : ICommandHandler<UpdateInterestCategoryCommand, UpdateInterestCategoryDTos>
{
    public async Task<ResultT<UpdateInterestCategoryDTos>> Handle(UpdateInterestCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var interestCategory = await EntityHelper.GetEntityByIdAsync(
            interestCategoryRepository.GetByIdAsync,
            request.InterestCategoryId,
            "Interest Category",
            logger);

        if (!interestCategory.IsSuccess)
            return ResultT<UpdateInterestCategoryDTos>.Failure(interestCategory.Error!);

        if (await interestCategoryRepository.ExistsNameExceptIdAsync(request.Name!, request.InterestCategoryId,
                cancellationToken))
        {
            logger.LogError("The name '{RequestName}' already exists in another Interest Category.", request.Name);

            return ResultT<UpdateInterestCategoryDTos>.Failure(
                Error.Failure("400", $"The name '{request.Name}' already exists."));
        }

        interestCategory.Value.Name = request.Name;
        interestCategory.Value.UpdateAt = DateTime.UtcNow;

        await interestCategoryRepository.UpdateAsync(interestCategory.Value, cancellationToken);

        logger.LogInformation(
            "Interest Category with name '{InterestCategoryName}' was successfully updated with ID {InterestCategoryId}.",
            interestCategory.Value.Name, interestCategory.Value.Id);

        return ResultT<UpdateInterestCategoryDTos>.Success(
            InterestCategoryMapper.MapUpdateInterestCategoryDTos(interestCategory.Value));
    }
}