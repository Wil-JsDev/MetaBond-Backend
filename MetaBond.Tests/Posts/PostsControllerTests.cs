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
    public void CreatePost_Tests()
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
            }
        };
        
        var imageUrl = "https://cdn.fakeapp.com/images/test-image.png";
        var postsDTos = new PostsDTos(
            PostsId:  Guid.NewGuid(),
            Title: command.Title,
            Content: command.Content,
            ImageUrl: imageUrl,
            CommunitiesId: command.CommunitiesId,
            CreatedAt: DateTime.UtcNow
        );
        
        var expectedResult = ResultT<PostsDTos>.Success(postsDTos);
        
        _mediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var postsController = new PostsController(_mediator.Object);

        // Act

        var resultController = postsController.AddPostsAsync(command, CancellationToken.None);
        
        // Assert
        
        Assert.NotNull(resultController);
    }

    [Fact]
    public void DeletePosts_Tests()
    {
        
        // Arrange

        DeletePostsCommand deletePostsCommand = new()
        {
            PostsId = Guid.NewGuid()
        };
        
        var expectedResult = ResultT<Guid>.Success(Guid.NewGuid());        

        _mediator.Setup(x => x.Send(deletePostsCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
            
        var postsController = new PostsController(_mediator.Object);
        
        // Act

        var resultController = postsController.DeletePostsAsync(deletePostsCommand.PostsId, CancellationToken.None);

        // Assert

        Assert.NotNull(resultController);
    }

    [Fact]
    public void GetPostsByIdCommunities_Tests()
    {
        
        // Arrange
        var communitiesQuery = new GetPostsByIdCommunitiesQuery()
        {
            PostsId = Guid.NewGuid()
        };

        IEnumerable<PostsWithCommunitiesDTos> communitiesDTosEnumerable = new List<PostsWithCommunitiesDTos>()
        {
            new PostsWithCommunitiesDTos
            (
                PostsId: Guid.NewGuid(),
                Title: "Tests Title",
                Content: "Tests Content",
                ImageUrl: "",
                Communities: new List<CommunitySummaryDto>(),
                CreatedAt: DateTime.UtcNow
            )
        };
        
        var expectedResult = ResultT<IEnumerable<PostsWithCommunitiesDTos>>.Success(communitiesDTosEnumerable);
        
        _mediator.Setup(x => x.Send(communitiesQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var postsController = new PostsController(_mediator.Object);
        
        // Act

        var resultController = postsController.GetDetailsPosts(communitiesQuery.PostsId, CancellationToken.None);

        // Assert
        Assert.NotNull(resultController);
    }

    [Fact]
    public void GetFilterTitlePosts_Tests()
    {
        
        // Arrange

        GetFilterTitlePostsQuery getFilterTitlePostsQuery = new()
        {
            CommunitiesId = Guid.NewGuid(),
            Title = "Tests Title"
        };

        IEnumerable<PostsDTos> postsDTosEnumerable = new List<PostsDTos>()
        {
            new PostsDTos
            (
                    PostsId:  Guid.NewGuid(),
                    Title: "Tests Title",
                    Content: "Tests Content",
                    ImageUrl: "Url",
                    CommunitiesId: Guid.NewGuid(),
                    CreatedAt: DateTime.UtcNow
            )
        };
        
        var expectedResult = ResultT<IEnumerable<PostsDTos>>.Success(postsDTosEnumerable);

        var postsController = new PostsController(_mediator.Object);
        
        // Act
    
        var resultController = postsController.FilterByTitleAsync(getFilterTitlePostsQuery.CommunitiesId,getFilterTitlePostsQuery.Title, CancellationToken.None);

        // Assert
        
        Assert.NotNull(resultController);
        
    }

    [Fact]
    public void GetPagedPosts_Tests()
    {
        
        // Arrange

        GetPagedPostsQuery pagedPostsQuery = new()
        {
            PageNumber = 1,
            PageSize = 10
        };


        PagedResult<PostsDTos> pagedResult = new()
        {
            CurrentPage = 2,
            Items = new List<PostsDTos>(),
            TotalItems = 2,
            TotalPages = 2
        };
        
        var expectedResult = ResultT<PagedResult<PostsDTos>>.Success(pagedResult);
        
        _mediator.Setup(x => x.Send(pagedPostsQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var postsController = new PostsController(_mediator.Object);
        
        // Act

        var resultController = postsController.GetPagedResultAsync(pagedPostsQuery.PageNumber, pagedPostsQuery.PageSize,CancellationToken.None);

        // Assert
        
        Assert.NotNull(resultController);
        
    }
    
}
