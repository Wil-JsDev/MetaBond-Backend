using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.User.Commands.ResetPassword;

public sealed class ResetPasswordUserCommand : ICommand<string>
{
    public Guid UserId { get; set; }

    public string? NewPassword { get; set; }

    public string? ConfirmNewPassword { get; set; }
}