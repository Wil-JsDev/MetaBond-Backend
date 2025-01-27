﻿using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Events.Query.FilterByDateRange
{
    public sealed class FilterByDateRangeEventsQuery : IQuery<IEnumerable<EventsDto>>
    {
        public DateRangeFilter DateRangeFilter { get; set; }
    }
}
