using MediatR;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Feature.Rewards.Commands.Create;
using MetaBond.Application.Feature.Rewards.Commands.Delete;
using MetaBond.Application.Feature.Rewards.Commands.Update;
using MetaBond.Application.Feature.Rewards.Query.GetById;
using MetaBond.Application.Feature.Rewards.Query.GetRange;
using MetaBond.Application.Feature.Rewards.Query.GetTop;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Domain.Models;
using MetaBond.Presentation.Api.Controllers.V1;
using Moq;

namespace MetaBond.Tests.Rewards;

public class RewardsControllerTests
{
    private readonly Mock<IMediator> _mediator = new();

    [Fact]
    public async Task CreateRewards_Test()
    {
        // Arrange
        var command = new CreateRewardsCommand
        {
            Description = "New Description",
            UserId = Guid.NewGuid(),
            PointAwarded = 12
        };

        var dto = new RewardsDTos(
            RewardsId: Guid.NewGuid(),
            UserId: command.UserId,
            Description: command.Description,
            PointAwarded: command.PointAwarded,
            DateAwarded: DateTime.UtcNow
        );

        var expected = ResultT<RewardsDTos>.Success(dto);

        _mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var controller = new RewardsController(_mediator.Object);

        // Act
        var result = await controller.AddAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Description, result.Value.Description);
        _mediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteRewards_Test()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new DeleteRewardsCommand { RewardsId = id };
        var expected = ResultT<Guid>.Success(id);

        _mediator.Setup(m => m.Send(It.Is<DeleteRewardsCommand>(c => c.RewardsId == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var controller = new RewardsController(_mediator.Object);

        // Act
        var result = await controller.DeleteAsync(id, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value);
        _mediator.Verify(m => m.Send(It.IsAny<DeleteRewardsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateRewards_Test()
    {
        // Arrange
        var command = new UpdateRewardsCommand
        {
            RewardsId = Guid.NewGuid(),
            Description = "Updated",
            PointAwarded = 20
        };

        var dto = new RewardsDTos(
            RewardsId: command.RewardsId,
            UserId: Guid.NewGuid(),
            Description: command.Description,
            PointAwarded: command.PointAwarded,
            DateAwarded: DateTime.UtcNow
        );

        var expected = ResultT<RewardsDTos>.Success(dto);

        _mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var controller = new RewardsController(_mediator.Object);

        // Act
        var result = await controller.UpdateAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.RewardsId, result.Value.RewardsId);
        _mediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdRewards_Test()
    {
        // Arrange
        var id = Guid.NewGuid();

        var dto = new RewardsDTos(
            RewardsId: id,
            UserId: Guid.NewGuid(),
            Description: "Description",
            PointAwarded: 12,
            DateAwarded: DateTime.UtcNow
        );

        var expected = ResultT<RewardsDTos>.Success(dto);

        _mediator.Setup(m => m.Send(
                It.Is<GetByIdRewardsQuery>(q => q.RewardsId == id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var controller = new RewardsController(_mediator.Object);

        // Act
        var result = await controller.GetByIdAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.RewardsId);

        _mediator.Verify(m => m.Send(
            It.Is<GetByIdRewardsQuery>(q => q.RewardsId == id),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRangeRewards_Test()
    {
        // Arrange
        var dto = new RewardsDTos(
            RewardsId: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            Description: "Description",
            PointAwarded: 12,
            DateAwarded: DateTime.UtcNow
        );

        var expected = ResultT<IEnumerable<RewardsDTos>>.Success(new[] { dto });

        _mediator.Setup(m => m.Send(
                It.Is<GetByDateRangeRewardQuery>(q => q.Range == DateRangeType.Month),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var controller = new RewardsController(_mediator.Object);

        // Act
        var result = await controller.GetDateRangeAsync(DateRangeType.Month, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        _mediator.Verify(m => m.Send(
            It.Is<GetByDateRangeRewardQuery>(q => q.Range == DateRangeType.Month),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTopRewards_Test()
    {
        // Arrange
        var dtos = new List<RewardsWithUserDTos>
        {
            new RewardsWithUserDTos(Guid.NewGuid(), new UserRewardsDTos(Guid.NewGuid(), "Carlos", "García"),
                "Meta semanal", 50, DateTime.UtcNow),
            new RewardsWithUserDTos(Guid.NewGuid(), new UserRewardsDTos(Guid.NewGuid(), "Ana", "Martínez"),
                "Usuario activo", 120, DateTime.UtcNow),
            new RewardsWithUserDTos(Guid.NewGuid(), new UserRewardsDTos(Guid.NewGuid(), "Luis", "Fernández"),
                "Apoyo comunidad", 80, DateTime.UtcNow)
        };

        var expected = ResultT<IEnumerable<RewardsWithUserDTos>>.Success(dtos);

        _mediator.Setup(m => m.Send(
                It.Is<GetTopRewardsQuery>(q => q.TopCount == 3),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var controller = new RewardsController(_mediator.Object);

        // Act
        var result = await controller.GetTopRewards(3, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Count());

        _mediator.Verify(m => m.Send(
            It.Is<GetTopRewardsQuery>(q => q.TopCount == 3),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}