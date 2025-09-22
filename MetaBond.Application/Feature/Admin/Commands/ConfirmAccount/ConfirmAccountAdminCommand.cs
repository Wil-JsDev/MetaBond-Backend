using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.Admin.Commands.ConfirmAccount;

public sealed class ConfirmAccountAdminCommand : ICommand<string>
{
    public Guid? AdminId { get; set; }
    public string? Code { get; set; }
}