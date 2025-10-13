using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Auth;

namespace MetaBond.Application.Feature.Authentication.Commands.RefreshTokenUser;

public sealed class RefreshTokenUserCommand : ICommand<AuthenticationResponse>
{
    public string? RefreshToken { get; set; }
}