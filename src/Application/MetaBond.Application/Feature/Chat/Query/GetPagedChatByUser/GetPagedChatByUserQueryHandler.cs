using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Chat.Query.GetPagedChatByUser;

internal sealed class GetPagedChatByUserQueryHandler(
    ILogger<GetPagedChatByUserQueryHandler> logger,
    IChatRepository chatRepository,
    IUserRepository userRepository,
    IDistributedCache cache
) : IQueryHandler<GetPagedChatByUserQuery, PagedResult<ChatDTos>>
{
    public async Task<ResultT<PagedResult<ChatDTos>>> Handle(GetPagedChatByUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId,
            "User",
            logger
        );

        if (!user.IsSuccess) return ResultT<PagedResult<ChatDTos>>.Failure(user.Error);

        var validationPagination =
            PaginationHelper.ValidatePagination<ChatDTos>(request.PageNumber, request.PageSize, logger);

        if (!validationPagination.IsSuccess) return ResultT<PagedResult<ChatDTos>>.Failure(validationPagination.Error);

        var key = $"paged-chat-by-user-{request.UserId}-{request.PageNumber}-{request.PageSize}";
        var result = await cache.GetOrCreateAsync(key, async () =>
        {
            var paged = await chatRepository.GetPagedChatByUserIdAsync(request.PageNumber, request.PageSize,
                request.UserId, cancellationToken);

            var items = paged.Items ?? [];

            var pagedChatByUserDto = items.Select(ChatMapper.MapToChatDTos).ToList();

            var pagedResult = new PagedResult<ChatDTos>
            {
                CurrentPage = paged.CurrentPage,
                Items = pagedChatByUserDto,
                TotalItems = paged.TotalItems,
                TotalPages = paged.TotalPages
            };

            return pagedResult;
        }, cancellationToken: cancellationToken);

        if (!result.Items.Any())
        {
            logger.LogWarning("No chats found for user with ID: {UserId}", request.UserId);

            return ResultT<PagedResult<ChatDTos>>.Failure(
                Error.Failure("404", "No chats found for user with ID"));
        }

        logger.LogInformation("Chats found for user with ID: {UserId}", request.UserId);

        return ResultT<PagedResult<ChatDTos>>.Success(result);
    }
}