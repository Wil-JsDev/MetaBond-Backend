using MetaBond.Application.Interfaces.Repository;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;

namespace MetaBond.Infrastructure.Persistence.Repository;

public class EventParticipationRepository(MetaBondContext context)
    : GenericRepository<EventParticipation>(context), IEventParticipationRepository;