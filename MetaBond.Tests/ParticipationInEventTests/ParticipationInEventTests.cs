using MediatR;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.DTOs.ParticipationInEvent;
using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Application.Feature.ParticipationInEvent.Commands.Create;
using MetaBond.Application.Feature.ParticipationInEvent.Commands.Update;
using MetaBond.Application.Feature.ParticipationInEvent.Query.GetEvents;
using MetaBond.Application.Feature.ParticipationInEvent.Query.Pagination;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Presentation.Api.Controllers.V1;
using Moq;

namespace MetaBond.Tests.ParticipationInEventTests;

public class ParticipationInEventTests
{
    private readonly Mock<IMediator> _mediator = new();


    [Fact]
    public void CreateParticipationInEvent_Tests()
    {
     
        // Arrange
        CreateParticipationInEventCommand createParticipationInEventCommand = new()
        {
            EventId = Guid.NewGuid()
        };

        ParticipationInEventDTos participationInEventDTos = new
        (
            ParticipationInEventId: Guid.NewGuid(),
            EventId: createParticipationInEventCommand.EventId
        );
        
        var expectedResult = ResultT<ParticipationInEventDTos>.Success(participationInEventDTos);
        
        _mediator.Setup(x => x.Send(createParticipationInEventCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var participationInEventController = new ParticipationInEventController(_mediator.Object);

        //Act
        var resultController = participationInEventController.CreateAsync(createParticipationInEventCommand, CancellationToken.None);
        //Assert
        Assert.NotNull(resultController);
    }

    [Fact]
    public void UpdateParticipationInEvent_Tests()
    {
        
        // Arrange

        UpdateParticipationInEventCommand updateParticipationInEventCommand = new()
        {
            Id = Guid.NewGuid(),
            EventId = Guid.NewGuid()
        };

        ParticipationInEventDTos participationInEventDTos = new
        (
            ParticipationInEventId: Guid.NewGuid(),
            EventId: updateParticipationInEventCommand.EventId
        );
        
        var expectedResult = ResultT<ParticipationInEventDTos>.Success(participationInEventDTos);
        
        _mediator.Setup(x => x.Send(updateParticipationInEventCommand, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);  
        
        var participationInEventController = new ParticipationInEventController(_mediator.Object);
        
        //Act

        var resultController = participationInEventController.UpdateAsync(updateParticipationInEventCommand, CancellationToken.None);

        // Assert
        
        Assert.NotNull(resultController);
    }

    [Fact]
    public void GetEventsParticipationInEvents_Tests()
    {
        
        // Arrange
        GetEventsQuery query = new()
        {
            ParticipationInEventId = Guid.NewGuid()
        };

        IEnumerable<EventsWithParticipationInEventDTos> eventsWithParticipationInEventDTosEnumerable =
            new List<EventsWithParticipationInEventDTos>()
            {
               new EventsWithParticipationInEventDTos
                (
                    ParticipationInEventId: query.ParticipationInEventId,
                    Events: new List<EventsDto>()
                )
            };
        
        var expectedResult = ResultT<IEnumerable<EventsWithParticipationInEventDTos>>.Success(eventsWithParticipationInEventDTosEnumerable);
        
        _mediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var participationInEventController = new ParticipationInEventController(_mediator.Object);
        
        // Act

        var resultController = participationInEventController.GetParticipationInEventDetailsAsync((Guid)query.ParticipationInEventId,CancellationToken.None);

        // Assert
        
        Assert.NotNull(resultController);
    }
    
    [Fact]
    public void GetPagedResultParticipationInEvents_Tests()
    {
        // Arrange
        GetPagedParticipationInEventQuery getPagedParticipationInEventQuery = new()
        {
            PageSize = 2,
            PageNumber = 1
        };

        PagedResult<ParticipationInEventDTos> pagedResult = new()
        {
            CurrentPage = 1,
            Items = new List<ParticipationInEventDTos>(),
            TotalItems = 1,
            TotalPages = 2
        };
        
        var expectedResult = ResultT<PagedResult<ParticipationInEventDTos>>.Success(pagedResult);
        
        _mediator.Setup(x => x.Send(getPagedParticipationInEventQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var participationInEventController = new ParticipationInEventController(_mediator.Object);

        // Act

        var resultController = participationInEventController.GetPagedAsync(getPagedParticipationInEventQuery.PageNumber, getPagedParticipationInEventQuery.PageSize, CancellationToken.None);
        
        // Assert
        
        Assert.NotNull(resultController);
        
    }
}