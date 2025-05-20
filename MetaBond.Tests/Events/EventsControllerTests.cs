using MediatR;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Feature.Events.Commands.Create;
using MetaBond.Application.Feature.Events.Commands.Delete;
using MetaBond.Application.Feature.Events.Commands.Update;
using MetaBond.Application.Feature.Events.Query.FilterByDateRange;
using MetaBond.Application.Feature.Events.Query.FilterByTitle;
using MetaBond.Application.Feature.Events.Query.FilterByTitleCommunityId;
using MetaBond.Application.Feature.Events.Query.GetById;
using MetaBond.Application.Feature.Events.Query.GetCommunitiesAndParticipationInEvent;
using MetaBond.Application.Feature.Events.Query.GetEventsWithParticipationInEvent;
using MetaBond.Application.Feature.Events.Query.GetOrderById;
using MetaBond.Application.Feature.Events.Query.Pagination;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Presentation.Api.Controllers.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using CancellationToken = System.Threading.CancellationToken;

namespace MetaBond.Tests.Events;

public class EventsControllerTests
{
    private readonly Mock<IMediator> _mediator = new();

    [Fact]
    public void CreateEvents_Tests()
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
        var resultController = eventsController.CreateAsync(createEventsCommand,CancellationToken.None);
        
        //Assert
        Assert.IsType<Task<IActionResult>>(resultController);
    }

    [Fact]
    public void DeleteEvents_Tests()
    {
        //Arrange
        DeleteEventsCommand deleteEventsCommand = new()
        {
            Id = Guid.NewGuid()
        };
        
        var resultId = Guid.NewGuid();

        var expectedResult = ResultT<Guid>.Success(resultId);
        
        _mediator.Setup(m => m.Send(deleteEventsCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultId);
        
        var eventsController = new EventsController(_mediator.Object);
        
        // Act
        var resultController = eventsController.DeleteAsync(deleteEventsCommand.Id, CancellationToken.None);

        //Assert
        Assert.NotNull(resultController);
    }

    [Fact]
    public void UpdateEvents_Tests()
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
            Id: Guid.NewGuid(),
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

        var resultController = eventsController.UpdateAsync(eventsCommand, CancellationToken.None);

        //Assert
        
        Assert.NotNull(resultController);
    }

    [Fact]
    public void FilterByDateRangeEvents_Tests()
    {
        // Arrange
        FilterByDateRangeEventsQuery filterByDateRangeEventsQuery = new()
        {
            CommunitiesId = Guid.NewGuid(),
            DateRangeFilter = DateRangeFilter.LastDay
        };

        IEnumerable<EventsDto> eventsDtosList = new List<EventsDto>()
        {
            new EventsDto
            (
                Id: Guid.NewGuid(),
                Description: "New Event",
                Title: "Event",
                DateAndTime: DateTime.Now,
                CreatedAt: DateTime.UtcNow,
                CommunitiesId: Guid.NewGuid()
            )
        };

        var expectedResult = ResultT<IEnumerable<EventsDto>>.Success(eventsDtosList);
        
        _mediator.Setup(m => m.Send(filterByDateRangeEventsQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var eventsController = new EventsController(_mediator.Object);
        
        //Act 
        var resultController = eventsController.FilterByRangeAsync(Guid.NewGuid(), DateRangeFilter.LastDay, CancellationToken.None);
        
        //Assert
        Assert.NotNull(resultController);
    }

    [Fact]
    public void FilterByTitleEvents_Tests()
    {
        
        //Arrange
        FilterByTitleEventsQuery eventsQuery = new()
        {
            Title = "New Title"   
        };
        
        
        IEnumerable<EventsDto> eventsDtosList = new List<EventsDto>()
        {
            new EventsDto
            (
                Id: Guid.NewGuid(),
                Description: "New Event",
                Title: "Event",
                DateAndTime: DateTime.Now,
                CreatedAt: DateTime.UtcNow,
                CommunitiesId: Guid.NewGuid()
            )
        };

        var expectedResult = ResultT<IEnumerable<EventsDto>>.Success(eventsDtosList);
        
        _mediator.Setup(m => m.Send(eventsQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var eventsController = new EventsController(_mediator.Object);
        
        //Act
        
        var resultController = eventsController.FilterByTitleAsync(eventsQuery.Title, CancellationToken.None);
        
        //Assert
        Assert.IsType<Task<IActionResult>>(resultController);
    }

    [Fact]
    public void FilterByTitleCommunityIdEvents_Tests()
    {
        
        //Arrange
        
        GetEventsByTitleAndCommunityIdQuery eventsByTitleAndCommunityIdQuery = new()
        {
            CommunitiesId = Guid.NewGuid(),
            Title = "New"
        };
        
        IEnumerable<EventsDto> eventsDtosList = new List<EventsDto>()
        {
            new EventsDto
            (
                Id: Guid.NewGuid(),
                Description: "New Event",
                Title: "Event",
                DateAndTime: DateTime.Now,
                CreatedAt: DateTime.UtcNow,
                CommunitiesId: Guid.NewGuid()
            )
        };
        
        var expectedResult = ResultT<IEnumerable<EventsDto>>.Success(eventsDtosList);
        
        _mediator.Setup(m => m.Send(eventsByTitleAndCommunityIdQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var eventsController = new EventsController(_mediator.Object);
        
        //Act

        var resultController = eventsController.GetEventsByTitleAndCommunityIdAsync(eventsByTitleAndCommunityIdQuery.CommunitiesId,eventsByTitleAndCommunityIdQuery.Title, CancellationToken.None);

        // Assert
        
        Assert.NotNull(resultController);
    }

    [Fact]
    public void GetByIdEvents_Tests()
    {
        //Arrange
        GetByIdEventsQuery eventsQuery = new GetByIdEventsQuery()
        {
            Id = Guid.NewGuid()
        };

        EventsDto eventsDto = new
        (
                  
            Id: eventsQuery.Id,
            Description: "New Event",
            Title: "Event",
            DateAndTime: DateTime.Now,
            CreatedAt: DateTime.UtcNow,
            CommunitiesId: Guid.NewGuid()  
        );
        
        var expectedResult = ResultT<EventsDto>.Success(eventsDto);
        
        _mediator.Setup(m => m.Send(eventsQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var eventsController = new EventsController(_mediator.Object);
        
        //Act
        
        var resultController = eventsController.GetByIdAsync(eventsQuery.Id, CancellationToken.None);
        
        //Assert
        Assert.NotNull(resultController);
        
    }

    [Fact]
    public void GetEventsDetails_Tests()
    {
        //Arrange
        GetEventsDetailsQuery eventsDetailsQuery = new()
        {
            Id = Guid.NewGuid()
        };

        IEnumerable<CommunitiesDTos> communitiesDTosEnumerable = new List<CommunitiesDTos>
        {
            new(
                Guid.NewGuid(),
                "Community of .NET Developers",
                ".NET Devs",
                DateTime.UtcNow.AddDays(-10),
                DateTime.UtcNow.AddMonths(-2),
                new List<CommunitySummaryDto>
                {
                    new("Group about ASP.NET Core", "Technology", DateTime.UtcNow.AddMonths(-5)),
                    new("Blazor Community", "Web Development", DateTime.UtcNow.AddMonths(-4))
                }
            ),
            new(
                Guid.NewGuid(),
                "Community of AI Enthusiasts",
                "AI Innovators",
                DateTime.UtcNow.AddDays(-20),
                DateTime.UtcNow.AddMonths(-6),
                new List<CommunitySummaryDto>
                {
                    new("Research in Machine Learning", "Data Science", DateTime.UtcNow.AddYears(-1)),
                    new("Real-world AI Applications", "Technology", DateTime.UtcNow.AddMonths(-3))
                }
            ),
        };
        
        var expectedResult = ResultT<IEnumerable<CommunitiesDTos>>.Success(communitiesDTosEnumerable);
        
        _mediator.Setup(m => m.Send(eventsDetailsQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var eventsController = new EventsController(_mediator.Object);
        
        //Act

        var resultController = eventsController.GetEventsWithCommunitiesAsync(eventsDetailsQuery.Id, CancellationToken.None);
        
        //Assert
        Assert.IsType<Task<IActionResult>>(resultController);
    }

    [Fact]
    public void GetEventsWithParticipationInEvents_Tests()
    {
        
        //Arrange
        GetEventsWithParticipationInEventQuery eventQuery = new()
        {
            EventsId = Guid.NewGuid()
        };

        IEnumerable<EventsWithParticipationInEventsDTos> enumerable = new List<EventsWithParticipationInEventsDTos>
        {
            new(
                Guid.NewGuid(),
                "Annual Tech Conference",
                "TechCon 2025",
                DateTime.UtcNow.AddMonths(2),
                new List<ParticipationInEventBasicDTos>
                {
                    new(Guid.NewGuid(), Guid.NewGuid()),
                    new(Guid.NewGuid(), Guid.NewGuid())
                },
                DateTime.UtcNow.AddMonths(-1)
            ),
            new(
                Guid.NewGuid(),
                "AI and Machine Learning Summit",
                "AI Summit 2025",
                DateTime.UtcNow.AddMonths(3),
                new List<ParticipationInEventBasicDTos>
                {
                    new(Guid.NewGuid(), Guid.NewGuid()),
                    new(Guid.NewGuid(), Guid.NewGuid())
                },
                DateTime.UtcNow.AddMonths(-2)
            )
        };
        
        var expectedResult = ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>.Success(enumerable);
        
        _mediator.Setup(m => m.Send(eventQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var eventsController = new EventsController(_mediator.Object);
        
        //Act

        var resultController = eventsController.GetEventsWithParticipationInEventAsync((Guid)eventQuery.EventsId,CancellationToken.None);

        //Assert
        Assert.NotNull(resultController);
    }

    [Fact]
    public void GetOrderByIdAscEvents_Tests()
    {
        
        //Arrange

        GetOrderByIdEventsQuery eventsQuery = new()
        {
            Order = "Asc"
        };
        
        IEnumerable<EventsDto> eventsDtosList = new List<EventsDto>()
        {
            new EventsDto
            (
                Id: Guid.NewGuid(),
                Description: "New Event",
                Title: "Event",
                DateAndTime: DateTime.UtcNow,
                CreatedAt: DateTime.UtcNow,
                CommunitiesId: Guid.NewGuid()
            ),
            new EventsDto
            (
                Id: Guid.NewGuid(),
                Description: "New Event 2",
                Title: "Event 2",
                DateAndTime: DateTime.UtcNow,
                CreatedAt: DateTime.UtcNow,
                CommunitiesId: Guid.NewGuid()
            )
        };
        
        var expectedResult = ResultT<IEnumerable<EventsDto>>.Success(eventsDtosList);

        _mediator.Setup(m => m.Send(eventsQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var eventsController = new EventsController(_mediator.Object);
        
        //Act

        var resultController = eventsController.OrderByIdAsync(eventsQuery.Order, CancellationToken.None);

        //Assert
        Assert.NotNull(resultController);
    }

    [Fact]
    public void GetPagedEvents_Tests()
    {
        //Arrange
        GetPagedEventsQuery eventsQuery = new()
        {
            PageNumber = 1,
            PageSize = 10
        };

        PagedResult<EventsDto> eventsDTosList = new()
        {
            CurrentPage = 1,
            TotalPages = 10,
            TotalItems = 10,
            Items = new List<EventsDto>()
        };
        
        var expectedResult = ResultT<PagedResult<EventsDto>>.Success(eventsDTosList);

        _mediator.Setup(m => m.Send(eventsQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var eventsController = new EventsController(_mediator.Object);
        
        // Act

        var resultController = eventsController.GetPagedAsync(eventsQuery.PageNumber, eventsQuery.PageSize, CancellationToken.None);

        //Assert
        Assert.NotNull(resultController);
    }
    
    
}