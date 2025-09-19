using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Events.Query.GetOrderById;

public sealed class GetOrderByIdEventsQuery : IQuery<PagedResult<EventsDto>>
{
    public string? Order { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}