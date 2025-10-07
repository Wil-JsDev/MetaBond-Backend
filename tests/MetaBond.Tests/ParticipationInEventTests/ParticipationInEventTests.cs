using System.Collections;
using MediatR;
using MetaBond.Application.DTOs.Events;
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
    public async Task CreateParticipationInEvent_Tests()
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

        // Act
        var resultController =
            await participationInEventController.CreateAsync(createParticipationInEventCommand, CancellationToken.None);

        // Assert
        Assert.NotNull(resultController);
        Assert.True(resultController.IsSuccess);
        Assert.Equal(createParticipationInEventCommand.EventId, resultController.Value.EventId);
    }

    [Fact]
    public async Task UpdateParticipationInEvent_Tests()
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

        // Act
        var resultController =
            await participationInEventController.UpdateAsync(updateParticipationInEventCommand, CancellationToken.None);

        // Assert
        Assert.NotNull(resultController);
        Assert.True(resultController.IsSuccess);
        Assert.Equal(updateParticipationInEventCommand.EventId, resultController.Value.EventId);
    }

    [Fact]
    public async Task GetEventsParticipationInEvents_Tests()
    {
        // Arrange
        GetEventsQuery query = new()
        {
            ParticipationInEventId = Guid.NewGuid()
        };

        PagedResult<EventsWithParticipationInEventDTos> eventsWithParticipationInEventDTosEnumerable = new()
        {
            CurrentPage = 1,
            Items = new List<EventsWithParticipationInEventDTos>()
            {
                new EventsWithParticipationInEventDTos
                (
                    ParticipationInEventId: query.ParticipationInEventId,
                    Events: new List<EventsDto>()
                )
            },
            TotalItems = 1,
            TotalPages = 2
        };

        var expectedResult =
            ResultT<PagedResult<EventsWithParticipationInEventDTos>>.Success(
                eventsWithParticipationInEventDTosEnumerable);

        _mediator.Setup(x => x.Send(It.IsAny<GetEventsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var participationInEventController = new ParticipationInEventController(_mediator.Object);

        // Act
        var resultController =
            await participationInEventController.GetParticipationInEventDetailsAsync((Guid)query.ParticipationInEventId,
                1, 2,
                CancellationToken.None);

        // Assert
        Assert.NotNull(resultController);
        Assert.True(resultController.IsSuccess);
        if (resultController.Value.Items != null) Assert.Single((IEnumerable)resultController.Value.Items);
    }

    [Fact]
    public async Task GetPagedResultParticipationInEvents_Tests()
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

        _mediator.Setup(x => x.Send(It.IsAny<GetPagedParticipationInEventQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var participationInEventController = new ParticipationInEventController(_mediator.Object);

        // Act
        var resultController = await participationInEventController.GetPagedAsync(
            getPagedParticipationInEventQuery.PageNumber, getPagedParticipationInEventQuery.PageSize,
            CancellationToken.None);

        // Assert
        Assert.NotNull(resultController);
        Assert.True(resultController.IsSuccess);
        Assert.Equal(1, resultController.Value.CurrentPage);
    }
}