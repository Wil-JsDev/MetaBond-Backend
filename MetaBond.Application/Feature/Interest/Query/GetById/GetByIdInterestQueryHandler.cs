using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Interest;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Interest.Query.GetById;

internal sealed class GetByIdInterestQueryHandler(
    ILogger<GetByIdInterestQueryHandler> logger,
    IInterestRepository interestRepository
) : IQueryHandler<GetByIdInterestQuery, InterestDTos>
{
    public async Task<ResultT<InterestDTos>> Handle(GetByIdInterestQuery request, CancellationToken cancellationToken)
    {
        if (request.InterestId is null || request.InterestId == Guid.Empty)
        {
            logger.LogWarning("Invalid InterestId provided in GetByIdInterestQuery");

            return ResultT<InterestDTos>.Failure(Error.Failure("400", "Invalid InterestId"));
        }

        var interest = await EntityHelper.GetEntityByIdAsync(
            interestRepository.GetByIdAsync,
            request.InterestId.Value,
            "Interest",
            logger
        );

        if (!interest.IsSuccess)
            return ResultT<InterestDTos>.Failure(interest.Error!);

        logger.LogInformation("Successfully retrieved Interest with Id: {InterestId}", request.InterestId);

        return ResultT<InterestDTos>.Success(InterestMapper.ModelToDto(interest.Value));
    }
}