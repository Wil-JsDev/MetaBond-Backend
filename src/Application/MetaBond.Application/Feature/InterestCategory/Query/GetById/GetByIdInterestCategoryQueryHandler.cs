using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.InterestCategory;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.InterestCategory.Query.GetById;

internal sealed class GetByIdInterestCategoryQueryHandler(
    ILogger<GetByIdInterestCategoryQueryHandler> logger,
    IInterestCategoryRepository interestCategoryRepository
) : IQueryHandler<GetByIdInterestCategoryQuery, InterestCategoryGeneralDTos>
{
    public async Task<ResultT<InterestCategoryGeneralDTos>> Handle(GetByIdInterestCategoryQuery request,
        CancellationToken cancellationToken)
    {
        if (request.InterestCategoryId == Guid.Empty)
        {
            logger.LogWarning("The Interest Category ID is empty.");

            return ResultT<InterestCategoryGeneralDTos>.Failure(Error.Failure("400", "Interest Category ID is empty."));
        }

        var interestCategory = await EntityHelper.GetEntityByIdAsync(
            interestCategoryRepository.GetByIdAsync,
            request.InterestCategoryId,
            "Interest Category",
            logger
        );

        if (!interestCategory.IsSuccess)
            return ResultT<InterestCategoryGeneralDTos>.Failure(interestCategory.Error!);

        logger.LogInformation("Interest Category with ID {InterestCategoryId} was successfully retrieved.",
            request.InterestCategoryId);

        return ResultT<InterestCategoryGeneralDTos>.Success(
            InterestCategoryMapper.MapInterestCategoryGeneralDTos(interestCategory.Value));
    }
}