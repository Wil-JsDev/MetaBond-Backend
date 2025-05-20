using MediatR;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Feature.Friendship.Commands.Create;
using MetaBond.Application.Feature.Friendship.Commands.Delete;
using MetaBond.Application.Feature.Friendship.Commands.Update;
using MetaBond.Application.Feature.Friendship.Query.GetCountByStatus;
using MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedAfter;
using MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedBefore;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Presentation.Api.Controllers.V1;
using Moq;

namespace MetaBond.Tests.Friendship;

public class FriendshipControllerTests
{
    private readonly Mock<IMediator> _mediator = new();

    [Fact]
    public void CreateFriendship_Tests()
    {
        //Arrange
        CreateFriendshipCommand createFriendshipCommand = new()
        {
            Status = Status.Accepted
        };

        FriendshipDTos friendshipDTos = new
        (
            FriendshipId: Guid.NewGuid(),
            Status: createFriendshipCommand.Status,
            RequesterId: createFriendshipCommand.RequesterId,
            AddresseeId: createFriendshipCommand.RequesterId,
            CreatedAt: DateTime.UtcNow
        );
        
        var expectedResult = ResultT<FriendshipDTos>.Success(friendshipDTos);
        
        _mediator.Setup(m => m.Send(createFriendshipCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var friendshipController = new FriendshipController(_mediator.Object);
        
        //Act

        var resultController = friendshipController.CreateAsync(createFriendshipCommand, CancellationToken.None);

        //Assert
        Assert.True(resultController != null);
    }

    [Fact]
    public void UpdateFriendship_Tests()
    {
        // Arrange

        UpdateFriendshipCommand updateFriendshipCommand = new()
        {
            Id = Guid.NewGuid(),
            Status = Status.Accepted
        };
        
        UpdateFriendshipDTos friendshipDTos = new
        (
           StatusFriendship:  updateFriendshipCommand.Status
        );

        var expectedResult = ResultT<UpdateFriendshipDTos>.Success(friendshipDTos);
        
        _mediator.Setup(m => m.Send(updateFriendshipCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var friendshipController = new FriendshipController(_mediator.Object);

        // Act

        var resultController = friendshipController.UpdateAsync(updateFriendshipCommand, CancellationToken.None);

        // Assert
        Assert.NotSame(expectedResult, resultController);
    }

    [Fact]
    public void DeleteFriendship_Tests()
    {
        // Arrange
        DeleteFriendshipCommand deleteFriendshipCommand = new()
        {
            Id = Guid.NewGuid()
        };
        
        var expectedResult = ResultT<Guid>.Success(deleteFriendshipCommand.Id);
        
        _mediator.Setup(m => m.Send(deleteFriendshipCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var friendshipController = new FriendshipController(_mediator.Object);
        
        // Act

        var resultController = friendshipController.DeleteAsync(deleteFriendshipCommand.Id, CancellationToken.None);

        // Assert

        Assert.True(resultController != null);
    }

    [Fact]
    public void GetCreatedAfterFriendship_Tests()
    {
        //Arrange

        GetCreatedAfterFriendshipQuery getCreatedAfterFriendshipQuery = new()
        {
            DateRange = DateRangeType.Today
        };

        IEnumerable<FriendshipDTos> friendshipDTosEnumerable = new List<FriendshipDTos>()
        {
            new  FriendshipDTos
            (
                FriendshipId: Guid.NewGuid(),
                Status: Status.Accepted,
                RequesterId: Guid.NewGuid(),
                AddresseeId: Guid.NewGuid(),
                CreatedAt: DateTime.UtcNow
            )
        };
        
        var expectedResult = ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTosEnumerable);

        _mediator.Setup(m => m.Send(It.IsAny<GetCreatedAfterFriendshipQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var friendshipController = new FriendshipController(_mediator.Object);
        
        //Act

        var resultController = friendshipController.GetAfterCreatedAsync(DateRangeType.Today,CancellationToken.None);

        //Assert
        Assert.NotNull(resultController);
    }


    [Fact]
    public void GetBeforeFriendship_Tests()
    {
        
        // Arrange

        GetCreatedBeforeFriendshipQuery createdBeforeFriendshipQuery = new()
        {
            PastDateRangeType = PastDateRangeType.BeforeToday
        };

        IEnumerable<FriendshipDTos> friendshipDTosEnumerable = new List<FriendshipDTos>()
        {
            new  FriendshipDTos
            (
                FriendshipId: Guid.NewGuid(),
                Status: Status.Accepted,
                RequesterId: Guid.NewGuid(),
                AddresseeId: Guid.NewGuid(),
                CreatedAt: DateTime.UtcNow
            )
        };
        
        var expectedResult = ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTosEnumerable);
        
        var friendshipController = new FriendshipController(_mediator.Object);
        
        // Act

        var resultController = friendshipController.GetBeforeCreatedAsync(PastDateRangeType.BeforeToday, CancellationToken.None);

        // Assert
        
        Assert.NotNull(resultController);
    }

    [Fact]
    public void GetCountByStatusFriendship_Tests()
    {
        // Arrange
        
        GetCountByStatusFriendshipQuery friendshipQuery = new()
        {
            Status = Status.Accepted
        };

        
        int value = 1;
        var expectedResult = ResultT<int>.Success(value);
        
        _mediator.Setup(m => m.Send(friendshipQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var friendshipController = new FriendshipController(_mediator.Object);
        
        // Act

        var resultController = friendshipController.FilterByStatus(Status.Accepted,CancellationToken.None);

        // Assert
        Assert.NotNull(resultController);
    }
}