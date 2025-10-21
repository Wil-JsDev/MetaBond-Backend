using MediatR;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Feature.ProgressBoard.Commands.Create;
using MetaBond.Application.Feature.ProgressBoard.Commands.Delete;
using MetaBond.Application.Feature.ProgressBoard.Commands.Update;
using MetaBond.Application.Feature.ProgressBoard.Query.GetProgressEntries;
using MetaBond.Application.Feature.ProgressBoard.Query.GetRange;
using MetaBond.Application.Feature.ProgressBoard.Query.GetRecent;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Domain.Models;
using MetaBond.Presentation.Api.Controllers.V1;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MetaBond.Tests.ProgressBoard;

public class ProgressBoardControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<ICurrentService> _currentService = new();

    [Fact]
    public async Task CreateProgressBoard_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var communitiesId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Mock the current user service (this was a hidden bug)
        _currentService.Setup(s => s.CurrentId).Returns(userId);

        var parameter = new CreateProgressBoardParameter(communitiesId);

        var dto = new ProgressBoardDTos(
            ProgressBoardId: Guid.NewGuid(),
            CommunitiesId: communitiesId, // Must match the ID from the parameter
            UserId: userId, // Must match the ID from the mocked service
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<ProgressBoardDTos>.Success(dto);

        _mediator
            .Setup(x => x.Send(
                It.Is<CreateProgressBoardCommand>(c =>
                    c.CommunitiesId == communitiesId && // Check against parameter's ID
                    c.UserId == userId), // Check against service's ID
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new ProgressBoardController(_mediator.Object, _currentService.Object);

        // Act
        var result = await controller.CreateAsync(parameter, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(dto.ProgressBoardId, result.Value.ProgressBoardId);

        _mediator.Verify(x => x.Send(
                It.Is<CreateProgressBoardCommand>(c =>
                    c.CommunitiesId == communitiesId &&
                    c.UserId == userId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateProgressBoard_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var progressBoardId = Guid.NewGuid();
        var communitiesId = Guid.NewGuid();
        var userId = Guid.NewGuid(); // Assume this comes from the current user service

        // Mock the current user service (the controller needs this)
        _currentService.Setup(s => s.CurrentId).Returns(userId);

        // Assuming the parameter constructor takes (ProgressBoardId, CommunitiesId)
        var parameter = new UpdateProgressBoardParameter(progressBoardId, communitiesId);

        var dto = new ProgressBoardDTos(
            ProgressBoardId: progressBoardId,
            CommunitiesId: communitiesId,
            UserId: userId, // This should come from the mocked service
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<ProgressBoardDTos>.Success(dto);

        _mediator.Setup(x => x.Send(
                It.Is<UpdateProgressBoardCommand>(cmd =>
                    cmd.ProgressBoardId == progressBoardId &&
                    cmd.CommunitiesId == communitiesId &&
                    cmd.UserId == userId), // Assuming the controller adds the UserId
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new ProgressBoardController(_mediator.Object, _currentService.Object);

        // Act
        var result = await controller.UpdateAsync(parameter, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(dto.ProgressBoardId, result.Value.ProgressBoardId);

        _mediator.Verify(x => x.Send(
            It.Is<UpdateProgressBoardCommand>(cmd =>
                cmd.ProgressBoardId == progressBoardId &&
                cmd.CommunitiesId == communitiesId &&
                cmd.UserId == userId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProgressBoard_Test()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new DeleteProgressBoardCommand { ProgressBoardId = id };

        var expectedResult = ResultT<Guid>.Success(id);

        _mediator.Setup(x => x.Send(It.IsAny<DeleteProgressBoardCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new ProgressBoardController(_mediator.Object, _currentService.Object);

        // Act
        var result = await controller.DeleteAsync(id, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value);
        _mediator.Verify(
            x => x.Send(It.Is<DeleteProgressBoardCommand>(c => c.ProgressBoardId == id), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}