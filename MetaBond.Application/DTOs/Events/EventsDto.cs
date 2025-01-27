using MetaBond.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.DTOs.Events
{
    public sealed record EventsDto
    (
        Guid? Id,
        string? Description,
        string? Title,
        DateTime? DateAndTime,
        DateTime? CreatedAt,
        Guid? CommunitiesId,
        Guid? ParticipationInEventId
    );
}
