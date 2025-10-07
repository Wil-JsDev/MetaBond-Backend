using MediatR;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Feature.Events.Commands.Create;
using MetaBond.Application.Feature.Events.Commands.Delete;
using MetaBond.Application.Feature.Events.Commands.Update;
using MetaBond.Application.Feature.Events.Query.FilterByDateRange;
using MetaBond.Application.Feature.Events.Query.FilterByTitle;
using MetaBond.Application.Feature.Events.Query.FilterByTitleCommunityId;
using MetaBond.Application.Feature.Events.Query.GetById;
using MetaBond.Application.Feature.Events.Query.GetCommunities;
using MetaBond.Application.Feature.Events.Query.GetEventsWithParticipationInEvent;
using MetaBond.Application.Feature.Events.Query.GetOrderById;
using MetaBond.Application.Feature.Events.Query.Pagination;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Presentation.Api.Controllers.V1;
using Moq;
using CancellationToken = System.Threading.CancellationToken;

namespace MetaBond.Tests.Events;

public class EventsControllerTests
{
    private readonly Mock<IMediator> _mediator = new();

    [Fact]
    public async Task CreateEvents_Tests()
    {
        //Arrange
        CreateEventsCommand createEventsCommand = new()
        {
            Description = "New Event",
            Title = "New Title",
            DateAndTime = DateTime.Now,
            CommunitiesId = Guid.NewGuid()
        };

        EventsDto eventsDto = new
        (
            Id: Guid.NewGuid(),
            Description: createEventsCommand.Description,
            Title: createEventsCommand.Title,
            DateAndTime: createEventsCommand.DateAndTime,
            CreatedAt: DateTime.Now,
            CommunitiesId: createEventsCommand.CommunitiesId
        );

        var expectedResult = ResultT<EventsDto>.Success(eventsDto);

        _mediator.Setup(m => m.Send(createEventsCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var eventsController = new EventsController(_mediator.Object);

        //Act
        var result = await eventsController.CreateAsync(createEventsCommand, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(eventsDto, result.Value);
    }

    [Fact]
    public async Task DeleteEvents_Tests()
    {
        //Arrange
        var id = Guid.NewGuid();
        var expectedResult = ResultT<Guid>.Success(id);

        _mediator.Setup(m => m.Send(It.Is<DeleteEventsCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var eventsController = new EventsController(_mediator.Object);

        // Act
        var result = await eventsController.DeleteAsync(id, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value);
    }

    [Fact]
    public async Task UpdateEvents_Tests()
    {
        //Arrange
        UpdateEventsCommand eventsCommand = new()
        {
            Id = Guid.NewGuid(),
            Description = "Description",
            Title = "Title update"
        };

        EventsDto eventsDto = new
        (
            Id: eventsCommand.Id,
            Description: eventsCommand.Description,
            Title: eventsCommand.Title,
            DateAndTime: DateTime.Now,
            CreatedAt: DateTime.Now,
            CommunitiesId: Guid.NewGuid()
        );

        var expectedResult = ResultT<EventsDto>.Success(eventsDto);

        _mediator.Setup(m => m.Send(eventsCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var eventsController = new EventsController(_mediator.Object);

        //Act
        var result = await eventsController.UpdateAsync(eventsCommand, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(eventsDto, result.Value);
    }

    [Fact]
    public async Task FilterByDateRangeEvents_Tests()
    {
        // Arrange
        var communitiesId = Guid.NewGuid();
        var dateRangeFilter = DateRangeFilter.LastDay;

        PagedResult<EventsDto> eventsDtosList = new()
        {
            CurrentPage = 1,
            Items = new List<EventsDto>()
            {
                new EventsDto
                (
                    Id: Guid.NewGuid(),
                    Description: "New Event",
                    Title: "Event",
                    DateAndTime: DateTime.Now,
                    CreatedAt: DateTime.UtcNow,
                    CommunitiesId: communitiesId
                )
            },
            TotalItems = 1,
            TotalPages = 1
        };

        var expectedResult = ResultT<PagedResult<EventsDto>>.Success(eventsDtosList);

        _mediator.Setup(m => m.Send(It.Is<FilterByDateRangeEventsQuery>(q =>
                    q.CommunitiesId == communitiesId && q.DateRangeFilter == dateRangeFilter),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var eventsController = new EventsController(_mediator.Object);

        //Act 
        var result =
            await eventsController.FilterByRangeAsync(communitiesId, dateRangeFilter, 1, 1, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(eventsDtosList, result.Value);
    }

    [Fact]
    public async Task FilterByTitleEvents_Tests()
    {
        //Arrange
        var title = "New Title";

        PagedResult<EventsDto> eventsDtosList = new()
        {
            CurrentPage = 1,
            Items = new List<EventsDto>()
            {
                new EventsDto
                (
                    Id: Guid.NewGuid(),
                    Description: "New Event",
                    Title: "Title",
                    DateAndTime: DateTime.Now,
                    CreatedAt: DateTime.UtcNow,
                    CommunitiesId: Guid.Empty
                )
            },
            TotalItems = 1,
            TotalPages = 1
        };

        var expectedResult = ResultT<PagedResult<EventsDto>>.Success(eventsDtosList);

        _mediator.Setup(m =>
                m.Send(It.Is<FilterByTitleEventsQuery>(q => q.Title == title), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var eventsController = new EventsController(_mediator.Object);

        //Act
        var result = await eventsController.FilterByTitleAsync(title, 1, 1, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(eventsDtosList, result.Value);
    }

    [Fact]
    public async Task FilterByTitleCommunityIdEvents_Tests()
    {
        //Arrange
        var communityId = Guid.NewGuid();
        var title = "New";

        PagedResult<EventsDto> eventsDtosList = new()
        {
            CurrentPage = 1,
            Items = new List<EventsDto>()
            {
                new EventsDto
                (
                    Id: Guid.NewGuid(),
                    Description: "New Event",
                    Title: "Title",
                    DateAndTime: DateTime.Now,
                    CreatedAt: DateTime.UtcNow,
                    CommunitiesId: communityId
                )
            },
            TotalItems = 1,
            TotalPages = 1
        };

        var expectedResult = ResultT<PagedResult<EventsDto>>.Success(eventsDtosList);

        _mediator.Setup(m => m.Send(It.Is<GetEventsByTitleAndCommunityIdQuery>(q =>
                q.CommunitiesId == communityId && q.Title == title), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var eventsController = new EventsController(_mediator.Object);

        //Act
        var result =
            await eventsController.GetEventsByTitleAndCommunityIdAsync(communityId, title, 1, 1,
                CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(eventsDtosList, result.Value);
    }

    [Fact]
    public async Task GetByIdEvents_Tests()
    {
        //Arrange
        var id = Guid.NewGuid();

        EventsDto eventsDto = new
        (
            Id: id,
            Description: "New Event",
            Title: "Event",
            DateAndTime: DateTime.Now,
            CreatedAt: DateTime.UtcNow,
            CommunitiesId: Guid.NewGuid()
        );

        var expectedResult = ResultT<EventsDto>.Success(eventsDto);

        _mediator.Setup(m => m.Send(It.Is<GetByIdEventsQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var eventsController = new EventsController(_mediator.Object);

        //Act
        var result = await eventsController.GetByIdAsync(id, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(eventsDto, result.Value);
    }

    [Fact]
    public async Task GetEventsDetails_Tests()
    {
        //Arrange
        var eventId = Guid.NewGuid();

        PagedResult<CommunitiesEventsDTos> communitiesDTosEnumerable = new()
        {
            CurrentPage = 1,
            Items = new List<CommunitiesEventsDTos>
            {
                new(
                    Guid.NewGuid(),
                    "Community of .NET Developers",
                    ".NET Devs",
                    DateTime.UtcNow.AddDays(-10),
                    DateTime.UtcNow.AddMonths(-2),
                    new List<CommunitySummaryDto>
                    {
                        new("Group about ASP.NET Core", DateTime.UtcNow.AddMonths(-5)),
                        new("Blazor Community", DateTime.UtcNow.AddMonths(-4))
                    }
                )
            },
            TotalItems = 1,
            TotalPages = 1
        };

        var expectedResult = ResultT<PagedResult<CommunitiesEventsDTos>>.Success(communitiesDTosEnumerable);

        _mediator.Setup(m => m.Send(It.Is<GetEventsDetailsQuery>(q => q.Id == eventId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var eventsController = new EventsController(_mediator.Object);

        //Act
        var result = await eventsController.GetEventsWithCommunitiesAsync(eventId, 1, 2, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(communitiesDTosEnumerable, result.Value);
    }

    [Fact]
    public async Task GetEventsWithParticipationInEvents_Tests()
    {
        //Arrange
        var eventId = Guid.NewGuid();

        PagedResult<EventsWithParticipationInEventsDTos> enumerable = new()
        {
            CurrentPage = 1,
            Items = new List<EventsWithParticipationInEventsDTos>
            {
                new(
                    eventId,
                    "Annual Tech Conference",
                    "TechCon 2025",
                    DateTime.UtcNow.AddMonths(2),
                    new List<ParticipationInEventBasicDTos>
                    {
                        new(Guid.NewGuid(), Guid.NewGuid()),
                        new(Guid.NewGuid(), Guid.NewGuid())
                    },
                    DateTime.UtcNow.AddMonths(-1)
                )
            },
            TotalItems = 1,
            TotalPages = 1
        };

        var expectedResult = ResultT<PagedResult<EventsWithParticipationInEventsDTos>>.Success(enumerable);

        _mediator.Setup(m => m.Send(It.Is<GetEventsWithParticipationInEventQuery>(q => q.EventsId == eventId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var eventsController = new EventsController(_mediator.Object);

        //Act
        var result =
            await eventsController.GetEventsWithParticipationInEventAsync(eventId, 1, 2, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(enumerable, result.Value);
    }

    [Fact]
    public async Task GetOrderByIdAscEvents_Tests()
    {
        //Arrange
        var order = "Asc";

        PagedResult<EventsDto> eventsDtosList = new PagedResult<EventsDto>()
        {
            CurrentPage = 1,
            Items = new List<EventsDto>()
            {
                new EventsDto
                (
                    Id: Guid.NewGuid(),
                    Description: "New Event",
                    Title: "Event",
                    DateAndTime: DateTime.UtcNow,
                    CreatedAt: DateTime.UtcNow,
                    CommunitiesId: Guid.NewGuid()
                )
            },
            TotalItems = 1,
            TotalPages = 1
        };

        var expectedResult = ResultT<PagedResult<EventsDto>>.Success(eventsDtosList);

        _mediator.Setup(m =>
                m.Send(It.Is<GetOrderByIdEventsQuery>(q => q.Order == order), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var eventsController = new EventsController(_mediator.Object);

        //Act
        var result = await eventsController.OrderByIdAsync(order, 1, 2, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(eventsDtosList, result.Value);
    }

    [Fact]
    public async Task GetPagedEvents_Tests()
    {
        //Arrange
        int pageNumber = 1;
        int pageSize = 10;

        PagedResult<EventsDto> eventsDTosList = new()
        {
            CurrentPage = pageNumber,
            TotalPages = 1,
            TotalItems = 2,
            Items = new List<EventsDto>
            {
                new EventsDto(
                    Id: Guid.NewGuid(),
                    Description: "Event 1",
                    Title: "Event 1",
                    DateAndTime: DateTime.UtcNow,
                    CreatedAt: DateTime.UtcNow,
                    CommunitiesId: Guid.NewGuid()
                ),
                new EventsDto(
                    Id: Guid.NewGuid(),
                    Description: "Event 2",
                    Title: "Event 2",
                    DateAndTime: DateTime.UtcNow,
                    CreatedAt: DateTime.UtcNow,
                    CommunitiesId: Guid.NewGuid()
                )
            }
        };

        var expectedResult = ResultT<PagedResult<EventsDto>>.Success(eventsDTosList);

        _mediator.Setup(m => m.Send(It.Is<GetPagedEventsQuery>(q =>
                q.PageNumber == pageNumber && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var eventsController = new EventsController(_mediator.Object);

        // Act
        var result = await eventsController.GetPagedAsync(pageNumber, pageSize, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(eventsDTosList, result.Value);
    }
}