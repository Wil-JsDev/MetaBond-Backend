using MediatR;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Feature.ProgressEntry.Commands.Create;
using MetaBond.Application.Feature.ProgressEntry.Commands.Delete;
using MetaBond.Application.Feature.ProgressEntry.Commands.Update;
using MetaBond.Application.Feature.ProgressEntry.Query.GetByIdProgressEntryWithProgressBoard;
using MetaBond.Application.Feature.ProgressEntry.Query.GetDateRange;
using MetaBond.Application.Feature.ProgressEntry.Query.GetRecent;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Domain.Models;
using MetaBond.Presentation.Api.Controllers.V1;
using Moq;

namespace MetaBond.Tests.ProgressEntry;

public class ProgressEntryControllerTests
{
    private readonly Mock<IMediator> _mediator = new();

    [Fact]
    public void CreateProgressEntry_Test()
    {
        
        // Arrange

        CreateProgressEntryCommand command = new()
        {
            Description = "Description",
            ProgressBoardId = Guid.NewGuid()
        };

        ProgressEntryDTos progressEntryDTos = new
        (
            ProgressEntryId: Guid.NewGuid(),
            ProgressBoardId: command.ProgressBoardId,
            UserId: command.UserId,
            Description: command.Description,
            CreatedAt: DateTime.UtcNow,
            UpdateAt: DateTime.UtcNow
        );
        
        var expectedResult = ResultT<ProgressEntryDTos>.Success(progressEntryDTos);
        
        _mediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var progressBoardController = new ProgressEntriesController(_mediator.Object);

        // Act

        var resultController = progressBoardController.CreateAsync(command, CancellationToken.None);

        // Assert
        
        Assert.NotNull(resultController);
        
    }


    [Fact]
    public void DeleteProgressEntry_Test()
    {
        
        // Arrange

        DeleteProgressEntryCommand command = new()
        {
            Id = Guid.NewGuid()
        };
        
        var expectedResult = ResultT<Guid>.Success(command.Id);
        
        _mediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var progressEntries = new ProgressEntriesController(_mediator.Object);

        
        // Act

        var resultController = progressEntries.DeleteAsync(command.Id, CancellationToken.None);

        // Assert

        Assert.NotNull(resultController);
    }

    [Fact]
    public void UpdateProgressEntry_Test()
    {
        
        // Arrange

        UpdateProgressEntryCommand command = new()
        {
            ProgressEntryId = Guid.NewGuid(),
            Description = "New description"
        };
        
        ProgressEntryDTos progressEntryDTos = new
        (
            ProgressEntryId: command.ProgressEntryId,
            ProgressBoardId: Guid.NewGuid(),
            UserId: Guid.NewGuid(), 
            Description: command.Description,
            CreatedAt: DateTime.UtcNow,
            UpdateAt: DateTime.UtcNow
        );
        
        var expectedResult = ResultT<ProgressEntryDTos>.Success(progressEntryDTos);
        
        _mediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);  
        
        var progressBoardController = new ProgressEntriesController(_mediator.Object);
        
        // Act

        var resultController = progressBoardController.UpdateAsync(command, CancellationToken.None);

        // Assert
        
        Assert.NotNull(resultController);
        
    }

    [Fact]
    public void GetProgressEntryWithBoard_Test()
    {
        
        // Arrange

        GetProgressEntryWithBoardByIdQuery query = new()
        {
            ProgressEntryId = Guid.NewGuid()
        };

        IEnumerable<ProgressEntryWithProgressBoardDTos> withProgressBoardDTosEnumerable =
            new List<ProgressEntryWithProgressBoardDTos>()
            {
                new ProgressEntryWithProgressBoardDTos
                (
                    ProgressEntryId: query.ProgressEntryId,
                    UserId: Guid.NewGuid(), 
                    ProgressBoard: new ProgressBoardSummaryDTos
                    (
                        ProgressBoardId: Guid.NewGuid(),
                        UserId:  Guid.NewGuid(),
                        CommunitiesId: Guid.NewGuid(),
                        CreatedAt: DateTime.UtcNow,
                        ModifiedAt: DateTime.UtcNow
                    ),
                    Description: "Description",
                    CreatedAt: DateTime.UtcNow,
                    UpdateAt: DateTime.UtcNow
                )
            };
        
        var expectedResult = ResultT<IEnumerable<ProgressEntryWithProgressBoardDTos>>.Success(withProgressBoardDTosEnumerable);

        _mediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var progressEntriesController = new ProgressEntriesController(_mediator.Object);
        
        // Act

        var resultController = progressEntriesController.GetProgressEntryWithBoard(query.ProgressEntryId,CancellationToken.None);

        // Assert
        
        Assert.NotNull(resultController);
        
    }

    [Fact]
    public void GetRecentProgressEntry_Test()
    {
        
        // Arrange
        GetRecentEntriesQuery query = new()
        {
            ProgressBoardId = Guid.NewGuid(),
            TopCount = 50
        };

        IEnumerable<ProgressEntryDTos> progressEntryDTosEnumerable = new List<ProgressEntryDTos>()
        {
            new ProgressEntryDTos
            (
                ProgressEntryId: Guid.NewGuid(),
                ProgressBoardId: query.ProgressBoardId,
                UserId: Guid.NewGuid(), 
                Description: "new description",
                CreatedAt: DateTime.UtcNow,
                UpdateAt: DateTime.UtcNow
            )
        };
        
        var expectedResult = ResultT<IEnumerable<ProgressEntryDTos>>.Success(progressEntryDTosEnumerable);
        
        _mediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var progressEntriesController = new ProgressEntriesController(_mediator.Object);
        
        // Act

        var resultController = progressEntriesController.GetFilterByRecentAsync(query.ProgressBoardId, query.TopCount, CancellationToken.None);

        // Assert

        Assert.NotNull(resultController);
        
    }

    [Fact]
    public void GetProgressEntryByDateRange_Test()
    {
        
        // Arrange

        GetEntriesByDateRangeQuery query = new()
        {
            ProgressBoardId = Guid.NewGuid(),
            Range = DateRangeType.Month
        };
        
        IEnumerable<ProgressEntryDTos> progressEntryDTosEnumerable = new List<ProgressEntryDTos>()
        {
            new ProgressEntryDTos
            (
                ProgressEntryId: Guid.NewGuid(),
                ProgressBoardId: query.ProgressBoardId,
                UserId: Guid.NewGuid(),
                Description: "new description",
                CreatedAt: DateTime.UtcNow,
                UpdateAt: DateTime.UtcNow
            )
        };

        var expectedResult = ResultT<IEnumerable<ProgressEntryDTos>>.Success(progressEntryDTosEnumerable);

        _mediator.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var progressEntriesController = new ProgressEntriesController(_mediator.Object);
        
        // Act

        var resultController = progressEntriesController.GetDateRangeAsync(query.ProgressBoardId,DateRangeType.Month,CancellationToken.None);

        // Assert
        
        Assert.NotNull(resultController);

    }
}