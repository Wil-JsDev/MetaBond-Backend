using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Commands.Create;

internal sealed class CreateProgressBoardCommandHandler(
    IProgressBoardRepository progressBoardRepository,
    IUserRepository userRepository,
    ICommunitiesRepository communitiesRepository,
    ILogger<CreateProgressBoardCommandHandler> logger)
    : ICommandHandler<CreateProgressBoardCommand, ProgressBoardDTos>
{
    public async Task<ResultT<ProgressBoardDTos>> Handle(
        CreateProgressBoardCommand request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId,
            "User",
            logger);

        if (!user.IsSuccess) return user.Error!;

        var community = await EntityHelper.GetEntityByIdAsync(
            communitiesRepository.GetByIdAsync,
            request.CommunitiesId,
            "Communities",
            logger
        );

        if (!community.IsSuccess) return community.Error!;

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