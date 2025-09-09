using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Commands.Delete;

internal sealed class DeleteProgressEntryCommandHandler(
    IProgressEntryRepository repository,
    ILogger<DeleteProgressEntryCommandHandler> logger)
    : ICommandHandler<DeleteProgressEntryCommand, Guid>
{
    public async Task<ResultT<Guid>> Handle(
        DeleteProgressEntryCommand request,
        CancellationToken cancellationToken)
    {
        var progressEntry = await EntityHelper.GetEntityByIdAsync
        (
            repository.GetByIdAsync,
            request.Id,
            "ProgressEntry",
            logger
        );

        if (progressEntry.IsSuccess)
        {
            await repository.DeleteAsync(progressEntry.Value, cancellationToken);

            logger.LogInformation("Progress entry with ID {Id} successfully deleted.", request.Id);

            return ResultT<Guid>.Success(request.Id);
        }

        logger.LogError("Progress entry with ID {Id} not found.", request.Id);

        return ResultT<Guid>.Failure(Error.NotFound("404", $"{request.Id} not found"));
    }
}