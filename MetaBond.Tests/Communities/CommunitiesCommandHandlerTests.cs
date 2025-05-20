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
using MetaBond.Domain.Models;
using MetaBond.Presentation.Api.Controllers.V1;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MetaBond.Tests.Communities;

public class CommunitiesCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();

    [Fact]
    public void CreateCommunitiesController()
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
            Category: createCommunitiesCommand.Category,
            CreatedAt: DateTime.UtcNow
        );
        
        var expectedResult = ResultT<CommunitiesDTos>.Success(communitiesDTos);

        _mediatorMock.Setup(m => m.Send(createCommunitiesCommand,It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var communitiesController = new CommunitiesController(_mediatorMock.Object);
        
        // Act
        var result = communitiesController.AddAsync(createCommunitiesCommand,CancellationToken.None);
        
        //Assert
         Assert.IsType<Task<IActionResult>>(result);
    }

    [Fact]
    public void DeleteCommunities_Tests()
    {
        //Arrange
        DeleteCommunitiesCommand communitiesCommand = new()
        {
            Id = Guid.NewGuid()
        };
        
        var resultId = Guid.NewGuid();
        
        var expectedResult = ResultT<Guid>.Success(resultId);
        
        _mediatorMock.Setup(m => m.Send(communitiesCommand,It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var communitiesController = new CommunitiesController(_mediatorMock.Object);

        //Act
        var result = communitiesController.DeleteAsync(Guid.NewGuid(),CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void UpdateCommunities_Tests()
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
            CommunitiesId: Guid.NewGuid(),
            Name: communitiesCommand.Name,
            Category: communitiesCommand.Category,
            CreatedAt: DateTime.UtcNow
        );
        
        var expectedResult = ResultT<CommunitiesDTos>.Success(communitiesDTos);
        
        _mediatorMock.Setup(m => m.Send(communitiesCommand,It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var communitiesController = new CommunitiesController(_mediatorMock.Object);
        
        //Act
        var result = communitiesController.UpdateAsync(communitiesCommand, CancellationToken.None);
        
        //Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetByCommunitiesId_Tests()
    {
        //Arrange
        GetByIdCommunitiesQuery idCommunitiesQuery = new()
        {
            Id = Guid.NewGuid()
        };
        
        CommunitiesDTos communitiesDTos = new
        (
            CommunitiesId: Guid.NewGuid(),
            Name: "New Name",
            Category: "New category",
            CreatedAt: DateTime.UtcNow
        );
        
        var expectedResult = ResultT<CommunitiesDTos>.Success(communitiesDTos);
        
        _mediatorMock.Setup(m => m.Send(idCommunitiesQuery,It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var communitiesController = new CommunitiesController(_mediatorMock.Object);
        
        //Act
        var result = communitiesController.GetByIdAsync(idCommunitiesQuery.Id,CancellationToken.None);

        //Assert
        Assert.IsType<Task<IActionResult>>(result);
    }

    [Fact]
    public void FilterByCategory_Tests()
    {
        
        //Arrange
        string Category = "New category";

        FilterCommunitiesQuery filterCommunitiesQuery = new()
        {
            Category = Category
        };
        
        IEnumerable<CommunitiesDTos> communitiesDTos = new List<CommunitiesDTos>
        {
            new CommunitiesDTos
            (
                CommunitiesId: Guid.NewGuid(),
                Name:"Another Community",
                Category: filterCommunitiesQuery.Category,
                CreatedAt:DateTime.UtcNow
            ),
            new CommunitiesDTos
            (
                CommunitiesId: Guid.NewGuid(),
                Name:"Another Community",
                Category: filterCommunitiesQuery.Category,
                CreatedAt:DateTime.UtcNow
            ),
            new CommunitiesDTos
            (
                CommunitiesId: Guid.NewGuid(),
                Name:"Another Community",
                Category: filterCommunitiesQuery.Category,
                CreatedAt:DateTime.UtcNow
            )
        };

        
        var expectedResult = ResultT<IEnumerable<CommunitiesDTos>>.Success(communitiesDTos);
        
        _mediatorMock.Setup(m => m.Send(filterCommunitiesQuery,It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var communitiesController = new CommunitiesController(_mediatorMock.Object);
        
        //Act
        var result = communitiesController.FilterByCategoryAsync(filterCommunitiesQuery.Category,CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetCommunitiesDetails_Tests()
    {
        
        //Arrange
        
        var query = new GetCommunityDetailsByIdQuery
        {
            Id = Guid.NewGuid(),
            PageNumber = 1,
            PageSize = 1
        };

        IEnumerable<PostsAndEventsDTos> postsAndEventsDTos = new List<PostsAndEventsDTos>
        {
            new PostsAndEventsDTos(
                CommunitiesId: Guid.NewGuid(),
                Name: "New Name",
                Category: "New Category",
                CreatedAt: DateTime.UtcNow,
                Posts: new List<Domain.Models.Posts>(),
                Events: new List<Domain.Models.Events>()
            ),
            new PostsAndEventsDTos(
                CommunitiesId: Guid.NewGuid(),
                Name: "Tech Talk",
                Category: "Technology",
                CreatedAt: DateTime.UtcNow,
                Posts: new List<Domain.Models.Posts>(),
                Events: new List<Domain.Models.Events>()
            ),
            new PostsAndEventsDTos(
                CommunitiesId: Guid.NewGuid(),
                Name: "Cooking 101",
                Category: "Cooking",
                CreatedAt: DateTime.UtcNow,
                Posts: new List<Domain.Models.Posts>(),
                Events: new List<Domain.Models.Events>()
            )
        };


        var expectedResult = ResultT<IEnumerable<PostsAndEventsDTos>>.Success(postsAndEventsDTos);
        
        _mediatorMock.Setup(m => m.Send(query,It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var controller = new CommunitiesController(_mediatorMock.Object);
        
        //Act
        var result = controller.GetCommunitiesDetailsAsync(query.Id,query.PageNumber,query.PageSize,CancellationToken.None);
        
        // Assert
        Assert.IsType<Task<IActionResult>>(result);
    }

    [Fact]
    public void GetPagedCommunities_Tests()
    {
        //Arrange
        
        var query = new GetPagedCommunitiesQuery
        {
            PageNumber = 1,
            PageSize = 1
        };

        PagedResult<CommunitiesDTos> pagedResult = new()
        {
            TotalItems = 2,
            CurrentPage = 1,
            TotalPages = 2,
            Items = new List<CommunitiesDTos>()
        };
        
        var expectedResult = ResultT<PagedResult<CommunitiesDTos>>.Success(pagedResult);
        
        _mediatorMock.Setup(m => m.Send(query,It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new CommunitiesController(_mediatorMock.Object);
        
        //Act
        
        var result = controller.GetPagedAsync(query.PageSize,query.PageNumber,CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
    }
}