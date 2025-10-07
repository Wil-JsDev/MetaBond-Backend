using MediatR;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Feature.ProgressEntry.Commands.Create;
using MetaBond.Application.Feature.ProgressEntry.Commands.Delete;
using MetaBond.Application.Feature.ProgressEntry.Commands.Update;
using MetaBond.Application.Feature.ProgressEntry.Query.GetByIdProgressEntryWithProgressBoard;
using MetaBond.Application.Feature.ProgressEntry.Query.GetDateRange;
using MetaBond.Application.Feature.ProgressEntry.Query.GetRecent;
using MetaBond.Application.Pagination;
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
    public async Task CreateProgressEntry_Test()
    {
        // Arrange
        var command = new CreateProgressEntryCommand
        {
            Description = "Description",
            ProgressBoardId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var dto = new ProgressEntryDTos(
            ProgressEntryId: Guid.NewGuid(),
            ProgressBoardId: command.ProgressBoardId,
            UserId: command.UserId,
            Description: command.Description,
            CreatedAt: DateTime.UtcNow,
            UpdateAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<ProgressEntryDTos>.Success(dto);

        _mediator
            .Setup(m => m.Send(It.Is<CreateProgressEntryCommand>(c =>
                    c.ProgressBoardId == command.ProgressBoardId && c.UserId == command.UserId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new ProgressEntriesController(_mediator.Object);

        // Act
        var result = await controller.CreateAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(dto.ProgressEntryId, result.Value.ProgressEntryId);
        _mediator.Verify(m => m.Send(It.IsAny<CreateProgressEntryCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateProgressEntry_Test()
    {
        // Arrange
        var command = new UpdateProgressEntryCommand
        {
            ProgressEntryId = Guid.NewGuid(),
            Description = "Updated description"
        };

        var dto = new ProgressEntryDTos(
            ProgressEntryId: command.ProgressEntryId,
            ProgressBoardId: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            Description: command.Description,
            CreatedAt: DateTime.UtcNow,
            UpdateAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<ProgressEntryDTos>.Success(dto);

        _mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new ProgressEntriesController(_mediator.Object);

        // Act
        var result = await controller.UpdateAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.ProgressEntryId, result.Value.ProgressEntryId);
        _mediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProgressEntry_Test()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new DeleteProgressEntryCommand { Id = id };

        var expectedResult = ResultT<Guid>.Success(id);

        _mediator.Setup(m => m.Send(It.IsAny<DeleteProgressEntryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new ProgressEntriesController(_mediator.Object);

        // Act
        var result = await controller.DeleteAsync(id, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value);
        _mediator.Verify(m => m.Send(It.Is<DeleteProgressEntryCommand>(c => c.Id == id), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetRecentProgressEntry_Test()
    {
        // Arrange
        var progressBoardId = Guid.NewGuid();
        var pageNumber = 1;
        var pageSize = 5;

        PagedResult<ProgressEntryDTos> pagedResult = new()
        {
            CurrentPage = pageNumber,
            Items = new List<ProgressEntryDTos>
            {
                new ProgressEntryDTos(
                    ProgressEntryId: Guid.NewGuid(),
                    ProgressBoardId: progressBoardId,
                    UserId: Guid.NewGuid(),
                    Description: "Recent entry",
                    CreatedAt: DateTime.UtcNow,
                    UpdateAt: DateTime.UtcNow
                )
            },
            TotalItems = 1,
            TotalPages = 1
        };

        var expectedResult = ResultT<PagedResult<ProgressEntryDTos>>.Success(pagedResult);

        _mediator.Setup(m => m.Send(
                It.Is<GetRecentEntriesQuery>(q =>
                    q.ProgressBoardId == progressBoardId &&
                    q.PageNumber == pageNumber &&
                    q.PageSize == pageSize),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new ProgressEntriesController(_mediator.Object);

        // Act
        var result =
            await controller.GetFilterByRecentAsync(progressBoardId, pageNumber, pageSize, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value.TotalItems);
        Assert.Equal(1, result.Value.TotalPages);
        Assert.Equal(pageNumber, result.Value.CurrentPage);
        Assert.Single(result.Value.Items);

        var firstItem = result.Value.Items.First();
        Assert.Equal(progressBoardId, firstItem.ProgressBoardId);

        _mediator.Verify(m => m.Send(
            It.Is<GetRecentEntriesQuery>(q =>
                q.ProgressBoardId == progressBoardId &&
                q.PageNumber == pageNumber &&
                q.PageSize == pageSize),
            It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task GetProgressEntryByDateRange_Test()
    {
        // Arrange
        var progressBoardId = Guid.NewGuid();

        PagedResult<ProgressEntryDTos> pagedResult = new()
        {
            CurrentPage = 1,
            Items = new List<ProgressEntryDTos>
            {
                new ProgressEntryDTos(
                    ProgressEntryId: Guid.NewGuid(),
                    ProgressBoardId: progressBoardId,
                    UserId: Guid.NewGuid(),
                    Description: "Recent entry",
                    CreatedAt: DateTime.UtcNow,
                    UpdateAt: DateTime.UtcNow
                )
            },
            TotalItems = 1,
            TotalPages = 1
        };

        var expectedResult = ResultT<PagedResult<ProgressEntryDTos>>.Success(pagedResult);

        _mediator.Setup(m => m.Send(
                It.Is<GetEntriesByDateRangeQuery>(q =>
                    q.ProgressBoardId == progressBoardId &&
                    q.Range == DateRangeType.Month),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new ProgressEntriesController(_mediator.Object);

        // Act
        // var result = await controller.GetDateRangeAsync(progressBoardId, DateRangeType.Month, CancellationToken.None);

        var result =
            await controller.GetDateRangeAsync(progressBoardId, 1, 2, DateRangeType.Month, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Items);
    }
}