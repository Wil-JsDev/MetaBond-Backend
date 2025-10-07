using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Interest;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Interest.Query.GetInterestsByName;

internal sealed class GetInterestByNameQueryHandler(
    ILogger<GetInterestByNameQueryHandler> logger,
    IInterestRepository interestRepository,
    IDistributedCache cache
) : IQueryHandler<GetInterestByNameQuery, PagedResult<InterestWithUserDto>>
{
    public async Task<ResultT<PagedResult<InterestWithUserDto>>> Handle(GetInterestByNameQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.InterestName))
        {
            logger.LogError("Interest name cannot be null or empty");

            return ResultT<PagedResult<InterestWithUserDto>>.Failure(Error.Failure("400", "Interest name is required"));
        }

        var validationPaginationResult =
            PaginationHelper.ValidatePagination<InterestWithUserDto>(request.PageNumber, request.PageSize, logger);
        if (!validationPaginationResult.IsSuccess)
            return validationPaginationResult.Error!;

        if (!await interestRepository.InterestExistsAsync(request.InterestName, cancellationToken))
        {
            logger.LogError("Interest with name {InterestName} not found", request.InterestName);
            return ResultT<PagedResult<InterestWithUserDto>>.Failure(Error.NotFound("404", "Interest not found"));
        }

        var result = await cache.GetOrCreateAsync(
            $"get-interest-by-name-{request.InterestName}-{request.PageNumber}-{request.PageSize}",
            async () =>
            {
                var paged = await interestRepository.GetInterestsByNameAsync(
                    request.InterestName, request.PageNumber, request.PageSize, cancellationToken);

                var dtoList = paged.Items!.Select(InterestMapper.UserWithUserDto).ToList();

                return new PagedResult<InterestWithUserDto>(
                    items: dtoList,
                    totalItems: paged.TotalItems,
                    currentPage: paged.CurrentPage,
                    pageSize: request.PageSize
                );
            },
            cancellationToken: cancellationToken);

        if (result.Items == null || !result.Items.Any())
        {
            logger.LogError("Interest with name {InterestName} not found", request.InterestName!);

            return ResultT<PagedResult<InterestWithUserDto>>.Failure(Error.NotFound("404", "Interest not found"));
        }

        logger.LogInformation("Interest with name {Name} retrieved successfully", request.InterestName);

        return ResultT<PagedResult<InterestWithUserDto>>.Success(result);
    }
}