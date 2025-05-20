using MediatR;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Feature.Rewards.Commands.Create;
using MetaBond.Application.Feature.Rewards.Commands.Delete;
using MetaBond.Application.Feature.Rewards.Commands.Update;
using MetaBond.Application.Feature.Rewards.Query.GetById;
using MetaBond.Application.Feature.Rewards.Query.GetRange;
using MetaBond.Application.Feature.Rewards.Query.GetTop;
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
    public void CreateRewards_Test()
    {
        
        // Arrange

        CreateRewardsCommand createRewardsCommand = new()
        {
            Description = "New Description",
            PointAwarded = 12
        };

        RewardsDTos rewardsDTos = new
        (
            RewardsId: Guid.NewGuid(),
            UserId: Guid.NewGuid(), 
            Description: createRewardsCommand.Description,
            PointAwarded: createRewardsCommand.PointAwarded,
            DateAwarded: DateTime.Now
        );
        
        var expectedResult = ResultT<RewardsDTos>.Success(rewardsDTos);

        _mediator.Setup(m => m.Send(createRewardsCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var rewardsController = new RewardsController(_mediator.Object);
        
        // Act
        
        var resultController = rewardsController.AddAsync(createRewardsCommand, CancellationToken.None);

        // Assert

        Assert.NotNull(resultController);
        
    }

    [Fact]
    public void DeleteRewards_Test()
    {
        
        // Arrange

        DeleteRewardsCommand command = new()
        {
            RewardsId = Guid.NewGuid()
        };

        var expectedResult = ResultT<Guid>.Success(command.RewardsId);
        
        _mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        
        var rewardsController = new RewardsController(_mediator.Object);

        // Act

        var resultController = rewardsController.DeleteAsync(command.RewardsId, CancellationToken.None);

        // Assert
        
        Assert.NotNull(resultController);
        
    }

    [Fact]
    public void UpdateRewards_Test()
    {
        
        // Arrange

        UpdateRewardsCommand command = new()
        {
            Description = "Description",
            PointAwarded = 12,
            RewardsId = Guid.NewGuid()
        };
        
        RewardsDTos rewardsDTos = new
        (
            RewardsId: command.RewardsId,
            UserId: Guid.NewGuid(),
            Description: command.Description,
            PointAwarded: command.PointAwarded,
            DateAwarded: DateTime.Now
        );
        
        var expectedResult = ResultT<RewardsDTos>.Success(rewardsDTos);
        
        _mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var rewardsController = new RewardsController(_mediator.Object);
        
        // Act
    
        var resultController = rewardsController.UpdateAsync(command, CancellationToken.None);

        // Assert
        
        Assert.NotNull(resultController);
        
    }

    [Fact]
    public void GetByIdRewards_Test()
    {
        
        // Arrange

        GetByIdRewardsQuery query = new()
        {
            RewardsId = Guid.NewGuid()
        };
        
        RewardsDTos rewardsDTos = new
        (
            RewardsId: query.RewardsId,
            UserId: Guid.NewGuid(),
            Description: "Description",
            PointAwarded: 12,
            DateAwarded: DateTime.Now
        );
        
        var expectedResult = ResultT<RewardsDTos>.Success(rewardsDTos);

        _mediator.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var rewardsController = new RewardsController(_mediator.Object);
        
        // Act

        var resulController = rewardsController.GetByIdAsync(query.RewardsId, CancellationToken.None);
        
        // Assert

        Assert.NotNull(resulController);
    }

    [Fact]
    public void GetRangeRewards_Test()
    {
        
        // Arrange

        GetByDateRangeRewardQuery query = new()
        {
            Range = DateRangeType.Month
        };
        
        IEnumerable<RewardsDTos> rewardsDTosEnumerable = new List<RewardsDTos>()
        {
            new RewardsDTos
            (
                RewardsId: Guid.NewGuid(),
                UserId:  Guid.NewGuid(),
                Description: "Description",
                PointAwarded: 12,
                DateAwarded: DateTime.Now
            )
        };

        var expectedResult = ResultT<IEnumerable<RewardsDTos>>.Success(rewardsDTosEnumerable);
        
        _mediator.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var rewardsController = new RewardsController(_mediator.Object);
        
        // Act

        var resultController = rewardsController.GetDateRangeAsync(query.Range, CancellationToken.None);

        // Assert
        
        Assert.NotNull(resultController);
        
    }

    [Fact]
    public void GetTopRewards_Test()
    {
        // Arrange

        GetTopRewardsQuery query = new()
        {
            TopCount = 12
        };
        
        IEnumerable<RewardsDTos> rewardsDTosEnumerable = new List<RewardsDTos>()
        {
            new RewardsDTos
            (
                RewardsId: Guid.NewGuid(),
                UserId: Guid.NewGuid(),
                Description: "Description",
                PointAwarded: 12,
                DateAwarded: DateTime.Now
            )
        };
        
        var expectedResult = ResultT<IEnumerable<RewardsDTos>>.Success(rewardsDTosEnumerable);
        
        _mediator.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var rewardsController = new RewardsController(_mediator.Object);
        
        // Act

        var resultController = rewardsController.GetTopRewards(query.TopCount, CancellationToken.None);
        
        // Assert
        
        Assert.NotNull(resultController);
        
    }
}
