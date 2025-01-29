using MetaBond.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.DTOs.Events
{
    public sealed record CommunitiesAndParticipationInEventDTos
    (
        Guid? Id,
        string? Description,
        string? Title,
        DateTime? DateAndTime,
        DateTime? CreatedAt,
        Communities Communities,
        ICollection<ParticipationInEvent> ParticipationInEvent
    );
}
