using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Events.Commands.Delete
{
    internal sealed class DeleteEventsCommandHandler : ICommandHandler<DeleteEventsCommand, Guid>
    {
        private readonly IEventsRepository _eventsRepository;

        public DeleteEventsCommandHandler(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task<ResultT<Guid>> Handle(DeleteEventsCommand request, CancellationToken cancellationToken)
        {
            var events = await _eventsRepository.GetByIdAsync(request.Id);
            if (events != null)
            {
                await _eventsRepository.DeleteAsync(events,cancellationToken);
                return ResultT<Guid>.Success(request.Id);

            }
            return ResultT<Guid>.Failure(Error.NotFound("404", $"{request.Id} not found"));
        }
    }
}
