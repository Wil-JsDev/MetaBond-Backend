using MediatR;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Feature.ProgressBoard.Commands.Create;
using MetaBond.Application.Feature.ProgressBoard.Commands.Delete;
using MetaBond.Application.Feature.ProgressBoard.Commands.Update;
using MetaBond.Application.Feature.ProgressBoard.Query.GetProgressEntries;
using MetaBond.Application.Feature.ProgressBoard.Query.GetRange;
using MetaBond.Application.Feature.ProgressBoard.Query.GetRecent;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Presentation.Api.Controllers.V1;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MetaBond.Tests.ProgressBoard;

public class ProgressBoardControllerTests
{
    private readonly Mock<IMediator> _mediator = new();

    [Fact]
    public void CreateProgressBoard_Test()
    {
        
        // Arrange

        CreateProgressBoardCommand createProgressBoardCommand = new()
        {
            CommunitiesId = Guid.NewGuid()
        };

        ProgressBoardDTos progressBoardDTos = new
        (
            ProgressBoardId: Guid.NewGuid(),
            CommunitiesId: createProgressBoardCommand.CommunitiesId,
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<ProgressBoardDTos>.Success(progressBoardDTos);
        
        _mediator.Setup(x => x.Send(createProgressBoardCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var progressBoardController = new ProgressBoardController(_mediator.Object);

        // Act

        var resultController = progressBoardController.CreateAsync(createProgressBoardCommand.CommunitiesId, CancellationToken.None);

        // Assert
        Assert.NotNull(resultController);
    }

    [Fact]
    public void UpdateProgressBoard_Test()
    {
        
        // Arrange

        UpdateProgressBoardCommand command = new()
        {
            ProgressBoardId = Guid.NewGuid(),
            CommunitiesId = Guid.NewGuid()
        };

        ProgressBoardDTos progressBoardDTos = new
        (
            ProgressBoardId: command.ProgressBoardId,
            CommunitiesId: command.CommunitiesId,
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<ProgressBoardDTos>.Success(progressBoardDTos);
        
        _mediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);  
        
        var progressBoardController = new ProgressBoardController(_mediator.Object);
        
        // Act

        var resultController =progressBoardController.UpdateAsync(command.ProgressBoardId,command, CancellationToken.None);

        // Assert

        Assert.NotNull(resultController);
    }

    [Fact]
    public void DeleteProgressBoard_Test()
    {
        
        // Arrange
        DeleteProgressBoardCommand command = new()
        {
            ProgressBoardId = Guid.NewGuid()
        };
        

        var expectedResult = ResultT<Guid>.Success(command.ProgressBoardId);
        
        _mediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var progressBoardController = new ProgressBoardController(_mediator.Object);
        
        // Act
            
        var resultController = progressBoardController.DeleteAsync(command.ProgressBoardId, CancellationToken.None);
        
        // Assert

        Assert.NotNull(resultController);
        
    }

    [Fact]
    public void GetProgressBoardWithEntries_Test()
    {
        
        // Arrange

        GetProgressBoardIdWithEntriesQuery query = new()
        {
            PageNumber = 1,
            PageSize = 10,
            ProgressBoardId = Guid.NewGuid()
        };

        IEnumerable<ProgressBoardWithProgressEntryDTos> progressBoardDTosEnumerable =
            new List<ProgressBoardWithProgressEntryDTos>()
            {
                new ProgressBoardWithProgressEntryDTos
                    (   
                        ProgressBoardId: query.ProgressBoardId,
                        CommunitiesId: Guid.NewGuid(),
                        ProgressEntries: new List<ProgressEntrySummaryDTos>(),
                        CreatedAt: DateTime.UtcNow,
                        UpdatedAt: DateTime.UtcNow
                    )
            };
        
        var progressBoardDTos = progressBoardDTosEnumerable.Select(x => new ProgressBoardWithProgressEntryDTos
            (
                ProgressBoardId: x.ProgressBoardId,
                CommunitiesId: x.CommunitiesId,
                ProgressEntries: x.ProgressEntries,
                CreatedAt: x.CreatedAt,
                UpdatedAt: x.UpdatedAt
            ));
        
        var expectedResult = ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Success(progressBoardDTos);
        
        _mediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);  
        
        var progressBoardController = new ProgressBoardController(_mediator.Object);
        
        // Act

        var resultController = progressBoardController.GetProgressEntriesAsync(query.ProgressBoardId,query.PageNumber, query.PageSize, CancellationToken.None);

        // Assert

        Assert.NotNull(resultController);

    }

    [Fact]
    public void GetRangeProgressEntries_Test()
    {
        
        // Arrange

        GetRangeProgressBoardQuery query = new()
        {
            Page = 1,
            PageSize = 10,
            DateRangeType = DateRangeType.Month
        };
        
        IEnumerable<ProgressBoardWithProgressEntryDTos> progressBoardDTosEnumerable =
            new List<ProgressBoardWithProgressEntryDTos>()
            {
                new ProgressBoardWithProgressEntryDTos
                (   
                    ProgressBoardId: Guid.NewGuid(),
                    CommunitiesId: Guid.NewGuid(),
                    ProgressEntries: new List<ProgressEntrySummaryDTos>(),
                    CreatedAt: DateTime.UtcNow,
                    UpdatedAt: DateTime.UtcNow
                )
            };
        
        var progressBoardDTos = progressBoardDTosEnumerable.Select(x => new ProgressBoardWithProgressEntryDTos
        (
            ProgressBoardId: x.ProgressBoardId,
            CommunitiesId: x.CommunitiesId,
            ProgressEntries: x.ProgressEntries,
            CreatedAt: x.CreatedAt,
            UpdatedAt: x.UpdatedAt
        ));
        
        var expectedResult = ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Success(progressBoardDTos);

        _mediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var progressBoardController = new ProgressBoardController(_mediator.Object);
        
        // Act

        var resultController = progressBoardController.GetFilterDateRangeAsync(
            DateRangeType.Month,
            query.PageSize,
            query.Page,
            CancellationToken.None);
        
        
        // Assert
        
        Assert.NotNull(resultController);
        
    }

    [Fact]
    public void GetRecentProgressBoard_Test()
    {
        
        // Arrange

        GetRecentProgressBoardQuery query = new()
        {
          DateFilter = DateRangeFilter.LastDay
        };
        
        IEnumerable<ProgressBoardDTos> progressBoardDTosEnumerable =
            new List<ProgressBoardDTos>()
            {
                new ProgressBoardDTos
                (   
                    ProgressBoardId: Guid.NewGuid(),
                    CommunitiesId: Guid.NewGuid(),
                    CreatedAt: DateTime.UtcNow,
                    UpdatedAt: DateTime.UtcNow
                )
            };

        var expectedResult = ResultT<IEnumerable<ProgressBoardDTos>>.Success(progressBoardDTosEnumerable);

        _mediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var progressBoardController = new ProgressBoardController(_mediator.Object);
        
        // Act

        var resultController = progressBoardController.GetFilterRecentAsync(query.DateFilter,CancellationToken.None);

        // Assert
        
        Assert.NotNull(resultController);
        
    }
    
}