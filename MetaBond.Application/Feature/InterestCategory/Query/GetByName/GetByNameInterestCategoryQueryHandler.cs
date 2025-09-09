using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.InterestCategory;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.InterestCategory.Query.GetByName;

internal sealed class GetByNameInterestCategoryQueryHandler(
    ILogger<GetByNameInterestCategoryQueryHandler> logger,
    IInterestCategoryRepository interestCategoryRepository
) : IQueryHandler<GetByNameInterestCategoryQuery, InterestCategoryGeneralDTos>
{
    public async Task<ResultT<InterestCategoryGeneralDTos>> Handle(GetByNameInterestCategoryQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            logger.LogWarning("Invalid Name provided in GetByNameInterestCategoryQuery");
            
            return ResultT<InterestCategoryGeneralDTos>.Failure(Error.Failure("400", "Invalid Name provided."));
        }

        var interestCategory = await interestCategoryRepository.GetByNameAsync(request.Name, cancellationToken);

        if (interestCategory is null)
        {
            logger.LogWarning("Interest Category with name {Name} does not exist.", request.Name);
            
            return ResultT<InterestCategoryGeneralDTos>.Failure(Error.NotFound("404", "Interest Category not found."));
        }

        logger.LogInformation("Interest Category with name {Name} was successfully retrieved.", request.Name);

        var dto = InterestCategoryMapper.MapInterestCategoryGeneralDTos(interestCategory);

        return ResultT<InterestCategoryGeneralDTos>.Success(dto);
    }
}