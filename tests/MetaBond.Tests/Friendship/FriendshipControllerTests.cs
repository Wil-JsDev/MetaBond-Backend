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
    public async Task CreateFriendship_Tests()
    {
        //Arrange
        CreateFriendshipCommand createFriendshipCommand = new()
        {
            RequesterId = Guid.NewGuid(),
            AddresseeId = Guid.NewGuid()
        };

        FriendshipDTos friendshipDTos = new(
            FriendshipId: Guid.NewGuid(),
            Status: Status.Accepted,
            RequesterId: createFriendshipCommand.RequesterId,
            AddresseeId: createFriendshipCommand.AddresseeId,
            CreatedAt: DateTime.UtcNow
        );

        var expectedResult = ResultT<FriendshipDTos>.Success(friendshipDTos);

        _mediator.Setup(m => m.Send(createFriendshipCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var friendshipController = new FriendshipController(_mediator.Object, _currentService.Object);

        var parameter = new AddresseeParameter(Guid.NewGuid());

        //Act
        var result = await friendshipController.CreateAsync(parameter, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(friendshipDTos, result.Value);
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