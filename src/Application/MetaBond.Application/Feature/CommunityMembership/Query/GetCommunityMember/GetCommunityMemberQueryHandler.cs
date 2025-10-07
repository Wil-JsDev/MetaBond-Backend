using System.Reflection.Metadata;
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.CommunityMembership;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.CommunityMembership.Query.GetCommunityMember;

internal sealed class GetCommunityMemberQueryHandler(
    ILogger<GetCommunityMemberQueryHandler> logger,
    ICommunityMembershipRepository communityMembershipRepository,
    IDistributedCache cache,
    ICommunitiesRepository communitiesRepository
) : IQueryHandler<GetCommunityMemberQuery, PagedResult<CommunityMembersDto>>

{
    public async Task<ResultT<PagedResult<CommunityMembersDto>>> Handle(GetCommunityMemberQuery request,
        CancellationToken cancellationToken)
    {
        var validationPagination = PaginationHelper.ValidatePagination<CommunityMembersDto>(request.PageNumber,
            request.PageSize, logger);

        if (!validationPagination.IsSuccess)
            return validationPagination;

        var communityResult = await EntityHelper.GetEntityByIdAsync(communitiesRepository.GetByIdAsync,
            request.CommunityId ?? Guid.Empty,
            "Community",
            logger
        );

        if (!communityResult.IsSuccess)
            return ResultT<PagedResult<CommunityMembersDto>>.Failure(communityResult.Error!);

        var communityMember = await cache.GetOrCreateAsync(
            $"get-community-member-{request.CommunityId}-p{request.PageNumber}-s{request.PageSize}",
            async () =>
            {
                var resultPaged = await communityMembershipRepository.GetCommunityMembersAsync(
                    request.CommunityId ?? Guid.Empty, request.PageNumber, request.PageSize, cancellationToken);

                var communityMembershipDtos = resultPaged.Items!
                    .Select(CommunityMembershipMapper.CommunityMembersToDto)
                    .ToList();

                return new PagedResult<CommunityMembersDto>
                {
                    TotalItems = resultPaged.TotalItems,
                    CurrentPage = resultPaged.CurrentPage,
                    TotalPages = resultPaged.TotalPages,
                    Items = communityMembershipDtos
                };
            },
            cancellationToken: cancellationToken);

        if (!communityMember.Items!.Any())
        {
            logger.LogWarning("Community with ID {CommunityId} has no members.", request.CommunityId);

            return ResultT<PagedResult<CommunityMembersDto>>.Failure(
                Error.NotFound("404", "No members found in this community.")
            );
        }

        logger.LogInformation("Retrieved {Count} members for community {CommunityId}, Page {PageNumber}/{TotalPages}",
            communityMember.Items!.Count(), request.CommunityId, communityMember.CurrentPage,
            communityMember.TotalPages);

        return ResultT<PagedResult<CommunityMembersDto>>.Success(communityMember);
    }
}