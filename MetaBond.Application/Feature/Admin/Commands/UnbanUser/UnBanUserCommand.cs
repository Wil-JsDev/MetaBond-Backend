using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Admin;

namespace MetaBond.Application.Feature.Admin.Commands.UnbanUser;

public sealed class UnBanUserCommand : ICommand<UnbanUserResultDto>
{
    public Guid? UserId { get; set; }
}