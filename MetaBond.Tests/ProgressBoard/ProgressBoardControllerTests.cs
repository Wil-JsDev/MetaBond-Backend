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
using MetaBond.Domain.Models;
using MetaBond.Presentation.Api.Controllers.V1;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MetaBond.Tests.ProgressBoard;

public class ProgressBoardControllerTests
{
    private readonly Mock<IMediator> _mediator = new();

    [Fact]
    public async Task CreateProgressBoard_Test()
    {
        // Arrange
        var command = new CreateProgressBoardCommand
        {
            CommunitiesId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var dto = new ProgressBoardDTos(
            ProgressBoardId: Guid.NewGuid(),
            CommunitiesId: command.CommunitiesId,
            UserId: command.UserId,
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<ProgressBoardDTos>.Success(dto);

        _mediator
            .Setup(x => x.Send(It.Is<CreateProgressBoardCommand>(c => c.CommunitiesId == command.CommunitiesId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new ProgressBoardController(_mediator.Object);

        // Act
        var result = await controller.CreateAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(dto.ProgressBoardId, result.Value.ProgressBoardId);
        _mediator.Verify(x => x.Send(It.IsAny<CreateProgressBoardCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateProgressBoard_Test()
    {
        // Arrange
        var command = new UpdateProgressBoardCommand
        {
            ProgressBoardId = Guid.NewGuid(),
            CommunitiesId = Guid.NewGuid()
        };

        var dto = new ProgressBoardDTos(
            ProgressBoardId: command.ProgressBoardId,
            CommunitiesId: command.CommunitiesId,
            UserId: command.UserId,
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<ProgressBoardDTos>.Success(dto);

        _mediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new ProgressBoardController(_mediator.Object);

        // Act
        var result = await controller.UpdateAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(dto.ProgressBoardId, result.Value.ProgressBoardId);
        _mediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
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

        var controller = new ProgressBoardController(_mediator.Object);

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