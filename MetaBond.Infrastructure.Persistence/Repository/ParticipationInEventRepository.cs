using MetaBond.Application.Interfaces.Repository;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Infrastructure.Persistence.Repository
{
    public class ParticipationInEventRepository : GenericRepository<ParticipationInEvent>, IParticipationInEventRepository
    {
        public ParticipationInEventRepository(MetaBondContext metaBondContext) : base(metaBondContext)
        {
        }
    }
}
