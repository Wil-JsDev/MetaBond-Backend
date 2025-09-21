using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Query.GetUsersByReward;

internal sealed class GetUsersByRewardIdQueryHandler(
    IRewardsRepository rewardsRepository,
    ILogger<GetUsersByRewardIdQueryHandler> logger,
    IDistributedCache decorated
) :
    IQueryHandler<GetUsersByRewardIdQuery,
        PagedResult<RewardsWithUserDTos>>
{
    public async Task<ResultT<PagedResult<RewardsWithUserDTos>>> Handle(
        GetUsersByRewardIdQuery request,
        CancellationToken cancellationToken)
    {
        var validationPagination =
            PaginationHelper.ValidatePagination<RewardsWithUserDTos>(request.PageNumber, request.PageSize, logger);
        if (!validationPagination.IsSuccess) return validationPagination.Error;

        var reward = await EntityHelper.GetEntityByIdAsync(
            rewardsRepository.GetByIdAsync,
            request.RewardsId,
            "Rewards",
            logger
        );

        if (!reward.IsSuccess)
            return reward.Error!;

        var rewardsWithUser = await decorated.GetOrCreateAsync(
            $"Get-User-By-Rewards-By-Id-{request.RewardsId}-page-{request.PageNumber}-size-{request.PageSize}",
            async () =>
            {
                var rewards = await rewardsRepository.GetUsersByRewardIdAsync(request.RewardsId, request.PageNumber,
                    request.PageSize, cancellationToken);

                var items = rewards.Items ?? [];
                var dtos = items.Select(RewardsMapper.RewardsWithUserToDto).ToList();

                PagedResult<RewardsWithUserDTos> resultPaged = new()
                {
                    TotalItems = rewards.TotalItems,
                    TotalPages = rewards.TotalPages,
                    CurrentPage = rewards.CurrentPage,
                    Items = dtos
                };

                return resultPaged;
            },
            cancellationToken: cancellationToken);

        var usersWithRewardList = rewardsWithUser.Items?.ToList() ?? [];
        if (!usersWithRewardList.Any())
        {
            logger.LogWarning("No users associated with reward ID {RewardId}. Page: {PageNumber}, Size: {PageSize}",
                request.RewardsId, request.PageNumber, request.PageSize);

            return ResultT<PagedResult<RewardsWithUserDTos>>.Failure(
                Error.NotFound("404", $"No users associated with reward ID {request.RewardsId}."));
        }

        logger.LogInformation(
            "Retrieved {Count} users associated with reward ID {RewardId}. Page: {PageNumber}, Size: {PageSize}",
            usersWithRewardList.Count, request.RewardsId, request.PageNumber, request.PageSize);

        return ResultT<PagedResult<RewardsWithUserDTos>>.Success(rewardsWithUser);
    }
}