using MediatR;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Feature.Friendship.Commands.Create;
using MetaBond.Application.Feature.Friendship.Commands.Delete;
using MetaBond.Application.Feature.Friendship.Commands.Update;
using MetaBond.Application.Feature.Friendship.Query.GetCountByStatus;
using MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedAfter;
using MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedBefore;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Presentation.Api.Controllers.V1;
using Moq;

namespace MetaBond.Tests.Friendship;

public class FriendshipControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<ICurrentService> _currentService = new();

    [Fact]
    public async Task CreateFriendship_ShouldReturnSuccess_WhenCommandIsValid()
    {
        //Arrange
        var requesterId = Guid.NewGuid();
        var addresseeId = Guid.NewGuid();

        // 1. Mock the current user service
        _currentService.Setup(s => s.CurrentId).Returns(requesterId);

        // 2. This is the parameter coming into the controller
        var parameter = new AddresseeParameter(addresseeId);

        // 3. This is the DTO we expect back from the mediator
        var friendshipDTos = new FriendshipDTos(
            FriendshipId: Guid.NewGuid(),
            Status: Status.Accepted,
            RequesterId: requesterId, // Should match the mocked service
            AddresseeId: addresseeId, // Should match the parameter
            CreatedAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<FriendshipDTos>.Success(friendshipDTos);

        // 4. Set up the mediator mock
        _mediator.Setup(m => m.Send(
                // Validate the command properties inside the mock
                It.Is<CreateFriendshipCommand>(cmd =>
                    cmd.RequesterId == requesterId &&
                    cmd.AddresseeId == addresseeId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var friendshipController = new FriendshipController(_mediator.Object, _currentService.Object);

        //Act
        var result = await friendshipController.CreateAsync(parameter, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess); // You should uncomment this
        Assert.NotNull(result.Value);
        Assert.Equal(friendshipDTos, result.Value);

        // (Optional) Verify the command was sent exactly once
        _mediator.Verify(m => m.Send(
            It.Is<CreateFriendshipCommand>(cmd =>
                cmd.RequesterId == requesterId &&
                cmd.AddresseeId == addresseeId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateFriendship_Tests()
    {
        // Arrange
        UpdateFriendshipCommand updateFriendshipCommand = new()
        {
            Id = Guid.NewGuid(),
            Status = Status.Accepted
        };

        UpdateFriendshipDTos friendshipDTos = new(
            StatusFriendship: updateFriendshipCommand.Status
        );

        var expectedResult = ResultT<UpdateFriendshipDTos>.Success(friendshipDTos);

        _mediator.Setup(m => m.Send(updateFriendshipCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var friendshipController = new FriendshipController(_mediator.Object, _currentService.Object);

        // Act
        var result = await friendshipController.UpdateAsync(updateFriendshipCommand, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(friendshipDTos, result.Value);
    }

    [Fact]
    public async Task DeleteFriendship_Tests()
    {
        // Arrange
        var friendshipId = Guid.NewGuid();

        var expectedResult = ResultT<Guid>.Success(friendshipId);

        _mediator.Setup(m =>
                m.Send(It.Is<DeleteFriendshipCommand>(cmd => cmd.Id == friendshipId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var friendshipController = new FriendshipController(_mediator.Object, _currentService.Object);

        // Act
        var result = await friendshipController.DeleteAsync(friendshipId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(friendshipId, result.Value);
    }

    [Fact]
    public async Task GetCreatedAfterFriendship_Tests()
    {
        //Arrange
        var dateRange = DateRangeType.Today;

        PagedResult<FriendshipDTos> friendshipDTosEnumerable = new()
        {
            CurrentPage = 1,
            Items = new List<FriendshipDTos>()
            {
                new FriendshipDTos(
                    FriendshipId: Guid.NewGuid(),
                    Status: Status.Accepted,
                    RequesterId: Guid.NewGuid(),
                    AddresseeId: Guid.NewGuid(),
                    CreatedAt: DateTime.UtcNow
                )
            },
            TotalItems = 1,
            TotalPages = 1
        };

        var expectedResult = ResultT<PagedResult<FriendshipDTos>>.Success(friendshipDTosEnumerable);

        _mediator.Setup(m => m.Send(It.Is<GetCreatedAfterFriendshipQuery>(q => q.DateRange == dateRange),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var friendshipController = new FriendshipController(_mediator.Object, _currentService.Object);

        //Act
        var result = await friendshipController.GetAfterCreatedAsync(dateRange, 1, 2, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(friendshipDTosEnumerable, result.Value);
    }

    [Fact]
    public async Task GetBeforeFriendship_Tests()
    {
        // Arrange
        var pastDateRange = PastDateRangeType.BeforeToday;

        PagedResult<FriendshipDTos> friendshipDTosEnumerable = new()
        {
            CurrentPage = 1,
            Items = new List<FriendshipDTos>()
            {
                new FriendshipDTos(
                    FriendshipId: Guid.NewGuid(),
                    Status: Status.Accepted,
                    RequesterId: Guid.NewGuid(),
                    AddresseeId: Guid.NewGuid(),
                    CreatedAt: DateTime.UtcNow
                )
            },
            TotalItems = 1,
            TotalPages = 1
        };

        var expectedResult = ResultT<PagedResult<FriendshipDTos>>.Success(friendshipDTosEnumerable);

        _mediator.Setup(m => m.Send(It.Is<GetCreatedBeforeFriendshipQuery>(q => q.PastDateRangeType == pastDateRange),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var friendshipController = new FriendshipController(_mediator.Object, _currentService.Object);

        // Act
        var result = await friendshipController.GetBeforeCreatedAsync(pastDateRange, 1, 2, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(friendshipDTosEnumerable, result.Value);
    }

    [Fact]
    public async Task GetCountByStatusFriendship_Tests()
    {
        // Arrange
        var status = Status.Accepted;
        GetCountByStatusFriendshipQuery friendshipQuery = new()
        {
            Status = status
        };

        int value = 1;
        var expectedResult = ResultT<int>.Success(value);

        _mediator.Setup(m => m.Send(It.Is<GetCountByStatusFriendshipQuery>(q => q.Status == status),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var friendshipController = new FriendshipController(_mediator.Object, _currentService.Object);

        // Act
        var result = await friendshipController.FilterByStatus(status, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
    }
}