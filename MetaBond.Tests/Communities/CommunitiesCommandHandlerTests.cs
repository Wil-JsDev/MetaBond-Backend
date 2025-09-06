using MediatR;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Feature.Communities.Commands.Create;
using MetaBond.Application.Feature.Communities.Commands.Delete;
using MetaBond.Application.Feature.Communities.Commands.Update;
using MetaBond.Application.Feature.Communities.Query.Filter;
using MetaBond.Application.Feature.Communities.Query.GetById;
using MetaBond.Application.Feature.Communities.Query.GetPostsAndEvents;
using MetaBond.Application.Feature.Communities.Query.Pagination;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Presentation.Api.Controllers.V1;
using Moq;

namespace MetaBond.Tests.Communities;

public class CommunitiesCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();

    [Fact]
    public async Task CreateCommunitiesController()
    {
        //Arrange
        CreateCommunitiesCommand createCommunitiesCommand = new()
        {
            Name = "Learning english",
            Description = "Learn English in 30 days",
            Category = "Learning"
        };

        CommunitiesDTos communitiesDTos = new
        (
            CommunitiesId: Guid.NewGuid(),
            Name: createCommunitiesCommand.Name,
            CreatedAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<CommunitiesDTos>.Success(communitiesDTos);

        _mediatorMock.Setup(m => m.Send(createCommunitiesCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var communitiesController = new CommunitiesController(_mediatorMock.Object);

        // Act
        var result = await communitiesController.AddAsync(createCommunitiesCommand, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(communitiesDTos, result.Value);
    }

    [Fact]
    public async Task DeleteCommunities_Tests()
    {
        //Arrange
        var id = Guid.NewGuid();
        var expectedResult = ResultT<Guid>.Success(id);

        _mediatorMock
            .Setup(m => m.Send(It.Is<DeleteCommunitiesCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var communitiesController = new CommunitiesController(_mediatorMock.Object);

        //Act
        var result = await communitiesController.DeleteAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value);
    }

    [Fact]
    public async Task UpdateCommunities_Tests()
    {
        //Arrange
        UpdateCommunitiesCommand communitiesCommand = new()
        {
            Id = Guid.NewGuid(),
            Name = "New Name",
            Category = "New Category",
        };

        CommunitiesDTos communitiesDTos = new
        (
            CommunitiesId: communitiesCommand.Id,
            Name: communitiesCommand.Name,
            CreatedAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<CommunitiesDTos>.Success(communitiesDTos);

        _mediatorMock.Setup(m => m.Send(communitiesCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var communitiesController = new CommunitiesController(_mediatorMock.Object);

        //Act
        var result = await communitiesController.UpdateAsync(communitiesCommand, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(communitiesDTos, result.Value);
    }

    [Fact]
    public async Task GetByCommunitiesId_Tests()
    {
        //Arrange
        var id = Guid.NewGuid();
        CommunitiesDTos communitiesDTos = new
        (
            CommunitiesId: id,
            Name: "New Name",
            CreatedAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<CommunitiesDTos>.Success(communitiesDTos);

        _mediatorMock.Setup(m => m.Send(It.Is<GetByIdCommunitiesQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var communitiesController = new CommunitiesController(_mediatorMock.Object);

        //Act
        var result = await communitiesController.GetByIdAsync(id, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(communitiesDTos, result.Value);
    }

    [Fact]
    public async Task FilterByCategoryAsync_ReturnsCommunities()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        int pageNumber = 1;
        int pageSize = 10;

        var communities = new List<CommunitiesDTos>
        {
            new CommunitiesDTos(Guid.NewGuid(), "Community 1", DateTime.UtcNow),
            new CommunitiesDTos(Guid.NewGuid(), "Community 2", DateTime.UtcNow),
            new CommunitiesDTos(Guid.NewGuid(), "Community 3", DateTime.UtcNow)
        };

        var pagedResult = new PagedResult<CommunitiesDTos>
        {
            TotalItems = communities.Count,
            CurrentPage = pageNumber,
            TotalPages = 1,
            Items = communities
        };

        var expectedResult = ResultT<PagedResult<CommunitiesDTos>>.Success(pagedResult);

        _mediatorMock.Setup(m =>
                m.Send(It.Is<GetCommunitiesByCategoryIdQuery>(q =>
                        q.CategoryId == categoryId &&
                        q.PageNumber == pageNumber &&
                        q.PageSize == pageSize),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var communitiesController = new CommunitiesController(_mediatorMock.Object);

        // Act
        var result =
            await communitiesController.GetCommunitiesByCategoryIdAsync(categoryId, pageNumber, pageSize,
                CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(pagedResult.TotalItems, result.Value.TotalItems);
        Assert.Equal(pagedResult.Items, result.Value.Items);
    }


    [Fact]
    public async Task GetCommunitiesDetails_Tests()
    {
        //Arrange
        var id = Guid.NewGuid();
        int pageNumber = 1;
        int pageSize = 10;

        IEnumerable<PostsAndEventsDTos> postsAndEventsDTos = new List<PostsAndEventsDTos>
        {
            new PostsAndEventsDTos(
                CommunitiesId: id,
                Name: "New Name",
                CreatedAt: DateTime.UtcNow,
                Posts: new List<Domain.Models.Posts>(),
                Events: new List<Domain.Models.Events>()
            ),
            new PostsAndEventsDTos(
                CommunitiesId: id,
                Name: "Tech Talk",
                CreatedAt: DateTime.UtcNow,
                Posts: new List<Domain.Models.Posts>(),
                Events: new List<Domain.Models.Events>()
            )
        };

        var expectedResult = ResultT<IEnumerable<PostsAndEventsDTos>>.Success(postsAndEventsDTos);

        _mediatorMock.Setup(m => m.Send(It.Is<GetCommunityDetailsByIdQuery>(q =>
                q.Id == id && q.PageNumber == pageNumber && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new CommunitiesController(_mediatorMock.Object);

        //Act
        var result = await controller.GetCommunitiesDetailsAsync(id, pageNumber, pageSize, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(postsAndEventsDTos, result.Value);
    }

    [Fact]
    public async Task GetPagedCommunities_Tests()
    {
        //Arrange
        int pageNumber = 1;
        int pageSize = 10;

        PagedResult<CommunitiesDTos> pagedResult = new()
        {
            TotalItems = 2,
            CurrentPage = pageNumber,
            TotalPages = 1,
            Items = new List<CommunitiesDTos>
            {
                new CommunitiesDTos(
                    CommunitiesId: Guid.NewGuid(),
                    Name: "Community 1",
                    CreatedAt: DateTime.UtcNow
                ),
                new CommunitiesDTos(
                    CommunitiesId: Guid.NewGuid(),
                    Name: "Community 2",
                    CreatedAt: DateTime.UtcNow
                )
            }
        };

        var expectedResult = ResultT<PagedResult<CommunitiesDTos>>.Success(pagedResult);

        _mediatorMock.Setup(m => m.Send(It.Is<GetPagedCommunitiesQuery>(q =>
                q.PageNumber == pageNumber && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new CommunitiesController(_mediatorMock.Object);

        //Act
        var result = await controller.GetPagedAsync(pageNumber, pageSize, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(pagedResult, result.Value);
    }
}