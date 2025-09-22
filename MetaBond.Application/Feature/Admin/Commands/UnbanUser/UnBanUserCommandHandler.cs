using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Admin;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Admin.Commands.UnbanUser;

internal sealed class UnBanUserCommandHandler(
    ILogger<UnBanUserCommandHandler> logger,
    IUserRepository userRepository,
    IAdminRepository adminRepository
) : ICommandHandler<UnBanUserCommand, UnbanUserResultDto>
{
    public async Task<ResultT<UnbanUserResultDto>> Handle(UnBanUserCommand request, CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId!.Value,
            "User",
            logger);

        if (!user.IsSuccess) return user.Error!;

        var userUnban = await adminRepository.UnbanUserAsync(request.UserId.Value, cancellationToken);

        logger.LogInformation("User {UserId} was successfully unbanned.", request.UserId);

        return ResultT<UnbanUserResultDto>.Success(AdminMapper.UnbanUserToDto(userUnban));
    }
}