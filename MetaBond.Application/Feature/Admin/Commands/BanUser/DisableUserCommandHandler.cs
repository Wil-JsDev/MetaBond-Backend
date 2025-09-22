using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Admin;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Admin.Commands.BanUser;

internal sealed class DisableUserCommandHandler(
    ILogger<DisableUserCommandHandler> logger,
    IUserRepository userRepository,
    IAdminRepository adminRepository
) : ICommandHandler<DisableUserCommand, BanUserResultDto>
{
    public async Task<ResultT<BanUserResultDto>> Handle(DisableUserCommand request, CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId!.Value,
            "User",
            logger);

        if (!user.IsSuccess) return user.Error!;

        var userBan = await adminRepository.BanUserAsync(request.UserId.Value, cancellationToken);

        logger.LogInformation("User {UserId} was successfully banned.", request.UserId);

        // Typo fix: BandUserToDTo -> BanUserToDto
        return ResultT<BanUserResultDto>.Success(AdminMapper.BandUserToDTo(userBan));
    }
}