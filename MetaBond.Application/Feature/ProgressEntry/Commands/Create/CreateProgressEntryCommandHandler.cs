using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Commands.Create;

internal sealed class CreateProgressEntryCommandHandler(
    IProgressEntryRepository progressEntryRepository,
    IUserRepository userRepository,
    IProgressBoardRepository progressBoardRepository,
    ILogger<CreateProgressEntryCommandHandler> logger)
    : ICommandHandler<CreateProgressEntryCommand, ProgressEntryDTos>
{
    public async Task<ResultT<ProgressEntryDTos>> Handle(
        CreateProgressEntryCommand request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId,
            "User",
            logger
        );

        if (!user.IsSuccess) return user.Error;

        var progressBoard = await EntityHelper.GetEntityByIdAsync(
            progressBoardRepository.GetByIdAsync,
            request.ProgressBoardId,
            "ProgressBoard",
            logger
        );

        if (!progressBoard.IsSuccess) return progressBoard.Error!;

        Domain.Models.ProgressEntry progressEntry = new()
        {
            Id = Guid.NewGuid(),
            ProgressBoardId = request.ProgressBoardId,
            UserId = request.UserId,
            Description = request.Description
        };

        await progressEntryRepository.CreateAsync(progressEntry, cancellationToken);

        logger.LogInformation("Progress entry created successfully with ID: {ProgressEntryId}", progressEntry.Id);

        var progressEntryDto = ProgressEntryMapper.ToDto(progressEntry);

        logger.LogInformation("Mapped ProgressEntry to DTO and returning success result.");

        return ResultT<ProgressEntryDTos>.Success(progressEntryDto);
    }
}