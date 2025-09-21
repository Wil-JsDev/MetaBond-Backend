using System.Collections;
using MediatR;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Feature.Posts.Commands.Create;
using MetaBond.Application.Feature.Posts.Commands.Delete;
using MetaBond.Application.Feature.Posts.Query.GetFilterTitle;
using MetaBond.Application.Feature.Posts.Query.GetPostByIdCommunities;
using MetaBond.Application.Feature.Posts.Query.Pagination;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Presentation.Api.Controllers.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace MetaBond.Tests.Posts;

public class PostsControllerTests
{
    private readonly Mock<IMediator> _mediator = new();

    [Fact]
    public async Task CreatePost_Tests()
    {
        // Arrange
        var command = new CreatePostsCommand
        {
            Title = "Tests Title",
            Content = "Tests Content",
            CommunitiesId = Guid.NewGuid(),
            ImageFile = new FormFile(
                baseStream: new MemoryStream(new byte[] { 1, 2, 3 }),
                baseStreamOffset: 0,
                length: 3,
                name: "image",
                fileName: "test-image.png")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            },
            CreatedById = Guid.NewGuid()
        };

        var postsDTos = new PostsDTos(
            PostsId: Guid.NewGuid(),
            Title: command.Title,
            Content: command.Content,
            ImageUrl: "https://cdn.fakeapp.com/images/test-image.png",
            CreatedById: command.CreatedById,
            CommunitiesId: command.CommunitiesId,
            CreatedAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<PostsDTos>.Success(postsDTos);

        _mediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var postsController = new PostsController(_mediator.Object);

        // Act
        var resultController = await postsController.AddPostsAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(resultController);
        Assert.True(resultController.IsSuccess);
        Assert.Equal(command.Title, resultController.Value.Title);
    }

    [Fact]
    public async Task DeletePosts_Tests()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var expectedResult = ResultT<Guid>.Success(postId);

        _mediator.Setup(x => x.Send(It.IsAny<DeletePostsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var postsController = new PostsController(_mediator.Object);

        // Act
        var resultController = await postsController.DeletePostsAsync(postId, CancellationToken.None);

        // Assert
        Assert.NotNull(resultController);
        Assert.True(resultController.IsSuccess);
        Assert.Equal(postId, resultController.Value);
    }

    [Fact]
    public async Task GetPostsByIdCommunities_Tests()
    {
        // Arrange
        var postId = Guid.NewGuid();
        PagedResult<PostsWithCommunitiesDTos> communitiesDTosEnumerable = new()
        {
            CurrentPage = 1,
            Items = new List<PostsWithCommunitiesDTos>
            {
                new PostsWithCommunitiesDTos(
                    PostsId: postId,
                    Title: "Tests Title",
                    Content: "Tests Content",
                    ImageUrl: "",
                    Communities: new List<CommunitySummaryDto>(),
                    CreatedAt: DateTime.UtcNow
                )
            },
            TotalItems = 1,
            TotalPages = 1
        };

        var expectedResult = ResultT<PagedResult<PostsWithCommunitiesDTos>>.Success(communitiesDTosEnumerable);

        _mediator.Setup(x => x.Send(It.IsAny<GetPostsByIdCommunitiesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var postsController = new PostsController(_mediator.Object);

        // Act
        var resultController = await postsController.GetDetailsPosts(postId, 1, 2, CancellationToken.None);

        // Assert
        Assert.NotNull(resultController);
        Assert.True(resultController.IsSuccess);
        if (resultController.Value.Items != null) Assert.Single((IEnumerable)resultController.Value.Items);
    }

    [Fact]
    public async Task GetFilterTitlePosts_Tests()
    {
        // Arrange
        var communityId = Guid.NewGuid();
        var postsDTosEnumerable = new PagedResult<PostsDTos>
        {
            CurrentPage = 1,
            Items = new List<PostsDTos>
            {
                new PostsDTos(
                    PostsId: Guid.NewGuid(),
                    Title: "Tests Title",
                    Content: "Tests Content",
                    ImageUrl: "Url",
                    CreatedById: Guid.NewGuid(),
                    CommunitiesId: communityId,
                    CreatedAt: DateTime.UtcNow
                )
            },
            TotalItems = 1,
            TotalPages = 1
        };

        var expectedResult = ResultT<PagedResult<PostsDTos>>.Success(postsDTosEnumerable);

        _mediator.Setup(x => x.Send(It.IsAny<GetFilterTitlePostsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var postsController = new PostsController(_mediator.Object);

        // Act
        var resultController =
            await postsController.FilterByTitleAsync(communityId, "Tests Title", 1, 2, CancellationToken.None);

        // Assert
        Assert.NotNull(resultController);
        Assert.True(resultController.IsSuccess);
    }

    [Fact]
    public async Task GetPagedPosts_Tests()
    {
        // Arrange
        var pagedPostsQuery = new GetPagedPostsQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        var pagedResult = new PagedResult<PostsDTos>
        {
            CurrentPage = 2,
            Items = new List<PostsDTos>(),
            TotalItems = 2,
            TotalPages = 2
        };

        var expectedResult = ResultT<PagedResult<PostsDTos>>.Success(pagedResult);

        _mediator.Setup(x => x.Send(It.IsAny<GetPagedPostsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var postsController = new PostsController(_mediator.Object);

        // Act
        var resultController = await postsController.GetPagedResultAsync(pagedPostsQuery.PageNumber,
            pagedPostsQuery.PageSize, CancellationToken.None);

        // Assert
        Assert.NotNull(resultController);
        Assert.True(resultController.IsSuccess);
        Assert.Equal(2, resultController.Value.CurrentPage);
    }
}