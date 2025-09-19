using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Commands.Create;

internal sealed class CreateProgressBoardCommandHandler(
    IProgressBoardRepository progressBoardRepository,
    ILogger<CreateProgressBoardCommandHandler> logger)
    : ICommandHandler<CreateProgressBoardCommand, ProgressBoardDTos>
{
    public async Task<ResultT<ProgressBoardDTos>> Handle(
        CreateProgressBoardCommand request,
        CancellationToken cancellationToken)
    {
        Domain.Models.ProgressBoard progressBoard = new()
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            CommunitiesId = request.CommunitiesId
        };

        await progressBoardRepository.CreateAsync(progressBoard, cancellationToken);

        logger.LogInformation("Progress board created successfully with ID: {ProgressBoardId}", progressBoard.Id);

        var progressBoardDTos = ProgressBoardMapper.ProgressBoardToDto(progressBoard);

        return ResultT<ProgressBoardDTos>.Success(progressBoardDTos);
    }
}