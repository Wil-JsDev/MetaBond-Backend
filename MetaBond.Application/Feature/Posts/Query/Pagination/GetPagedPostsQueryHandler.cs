using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.Pagination;

internal class GetPagedPostsQueryHandler(IPostsRepository postsRepository, ILogger<GetPagedPostsQueryHandler> logger)
    : IQueryHandler<GetPagedPostsQuery, PagedResult<PostsDTos>>
{
    public async Task<ResultT<PagedResult<PostsDTos>>> Handle(GetPagedPostsQuery request, CancellationToken cancellationToken)
    {
        if (request != null)
        {
            var pagedPosts = await postsRepository.GetPagedPostsAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            var postsDto = pagedPosts.Items!.Select(x => new PostsDTos
            (
                PostsId: x.Id,
                Title: x.Title,
                Content: x.Content,
                ImageUrl: x.Image,
                CommunitiesId: x.CommunitiesId,
                CreatedAt: x.CreatedAt
            ));

            IEnumerable<PostsDTos> postsDTosEnumerable = postsDto.ToList();
            if (!postsDTosEnumerable.Any())
            {
                logger.LogError("No posts found for the requested page: {PageNumber}, PageSize: {PageSize}", 
                    request.PageNumber, request.PageSize);

                return ResultT<PagedResult<PostsDTos>>.Failure(Error.Failure("400", ""));
            }

            PagedResult<PostsDTos> result = new()
            {
                TotalItems = pagedPosts.TotalItems,
                TotalPages = pagedPosts.TotalPages,
                CurrentPage = pagedPosts.CurrentPage,
                Items = postsDTosEnumerable
            };

            logger.LogInformation("Retrieved {TotalItems} posts for page {CurrentPage} of {TotalPages}.", 
                pagedPosts.TotalItems, pagedPosts.CurrentPage, pagedPosts.TotalPages);

            return ResultT<PagedResult<PostsDTos>>.Success(result);
        }
        logger.LogError("Invalid request: GetPagedPostsQuerys request is null.");


        return ResultT<PagedResult<PostsDTos>>.Failure(Error.Failure("400",""));
    }
}