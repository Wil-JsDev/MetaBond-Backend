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
        var topCount = 5;

        var dto = new ProgressEntryDTos(
            ProgressEntryId: Guid.NewGuid(),
            ProgressBoardId: progressBoardId,
            UserId: Guid.NewGuid(),
            Description: "Recent entry",
            CreatedAt: DateTime.UtcNow,
            UpdateAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<IEnumerable<ProgressEntryDTos>>.Success(new[] { dto });

        _mediator.Setup(m => m.Send(
                It.Is<GetRecentEntriesQuery>(q => q.ProgressBoardId == progressBoardId && q.TopCount == topCount),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new ProgressEntriesController(_mediator.Object);

        // Act
        var result = await controller.GetFilterByRecentAsync(progressBoardId, topCount, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal(progressBoardId, result.Value.First().ProgressBoardId);

        _mediator.Verify(m => m.Send(
            It.Is<GetRecentEntriesQuery>(q => q.ProgressBoardId == progressBoardId && q.TopCount == topCount),
            It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task GetProgressEntryByDateRange_Test()
    {
        // Arrange
        var progressBoardId = Guid.NewGuid();

        var dto = new ProgressEntryDTos(
            ProgressEntryId: Guid.NewGuid(),
            ProgressBoardId: progressBoardId,
            UserId: Guid.NewGuid(),
            Description: "Monthly entry",
            CreatedAt: DateTime.UtcNow,
            UpdateAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<IEnumerable<ProgressEntryDTos>>.Success(new[] { dto });

        _mediator.Setup(m => m.Send(
                It.Is<GetEntriesByDateRangeQuery>(q =>
                    q.ProgressBoardId == progressBoardId &&
                    q.Range == DateRangeType.Month),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new ProgressEntriesController(_mediator.Object);

        // Act
        var result = await controller.GetDateRangeAsync(progressBoardId, DateRangeType.Month, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }
}