using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.User.Commands.ForgotPassword;

public sealed class ForgotPasswordUserCommand : ICommand<string>
{
    public Guid? UserId { get; set; }
}