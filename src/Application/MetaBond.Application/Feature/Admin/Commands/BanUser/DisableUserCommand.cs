using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Admin;

namespace MetaBond.Application.Feature.Admin.Commands.BanUser;

public sealed class DisableUserCommand : ICommand<BanUserResultDto>
{
    public Guid? UserId { get; set; }
}