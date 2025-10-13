using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Auth;

namespace MetaBond.Application.Feature.Authentication.Commands.LoginUser;

public sealed class LoginUserCommand : ICommand<AuthenticationResponse>
{
    public string? Email { get; set; }

    public string? Password { get; set; }
}