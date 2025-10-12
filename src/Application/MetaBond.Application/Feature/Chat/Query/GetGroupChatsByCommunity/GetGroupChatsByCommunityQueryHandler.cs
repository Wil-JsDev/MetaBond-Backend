using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MetaBond.Application.Feature.Chat.Query.GetGroupChatsByCommunity;

internal sealed class GetGroupChatsByCommunityQueryHandler(
    ILogger<GetGroupChatsByCommunityQueryHandler> logger,
    IChatRepository chatRepository,
    IUserRepository userRepository,
    ICommunitiesRepository communitiesRepository,
    IDistributedCache cache
) : IQueryHandler<GetGroupChatsByCommunityQuery, PagedResult<ChatDTos>>
{
    public async Task<ResultT<PagedResult<ChatDTos>>> Handle(GetGroupChatsByCommunityQuery request,
        CancellationToken cancellationToken)
    {
        var community = await EntityHelper.GetEntityByIdAsync(
            communitiesRepository.GetByIdAsync,
            request.CommunityId,
            "Community",
            logger);

        if (!community.IsSuccess) return ResultT<PagedResult<ChatDTos>>.Failure(community.Error!);

        var pagination = PaginationHelper.ValidatePagination<ChatDTos>(request.PageNumber, request.PageSize, logger);

        if (!pagination.IsSuccess) return ResultT<PagedResult<ChatDTos>>.Failure(pagination.Error);

        var key = $"paged-chat-by-community-{request.CommunityId}-{request.PageNumber}-{request.PageSize}";

        // Validate if community has group chat
        if (!await chatRepository.IsCommunityGroupChatExistAsync(request.CommunityId, cancellationToken))
        {
            logger.LogWarning("No chats found for community with ID: {CommunityId}", request.CommunityId);

            return ResultT<PagedResult<ChatDTos>>.Failure(
                Error.Failure("404", "No chats found for community with ID"));
        }

        var result = await cache.GetOrCreateAsync(key, async () =>
        {
            var getGroupChat = await chatRepository.GetGroupChatsByCommunityIdAsync(request.CommunityId,
                request.PageNumber, request.PageSize, cancellationToken);

            var items = getGroupChat.Items ?? [];

            var groupChatDTos = items.Select(ChatMapper.MapToChatDTos);

            var pagedResult = new PagedResult<ChatDTos>
            {
                CurrentPage = getGroupChat.CurrentPage,
                Items = groupChatDTos,
                TotalItems = getGroupChat.TotalItems,
                TotalPages = getGroupChat.TotalPages
            };

            return pagedResult;
        }, cancellationToken: cancellationToken);

        if (!result.Items.Any())
        {
            logger.LogWarning("No chats found for community with ID: {CommunityId}", request.CommunityId);

            return ResultT<PagedResult<ChatDTos>>.Failure(
                Error.Failure("404", "No chats found for community with ID"));
        }

        logger.LogInformation("Chats found for community with ID: {CommunityId}", request.CommunityId);

        return ResultT<PagedResult<ChatDTos>>.Success(result);
    }
}