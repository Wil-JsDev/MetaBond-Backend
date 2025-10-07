using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Auth;

namespace MetaBond.Application.Feature.Authentication.Commands.RefreshTokenAdmin;

public sealed class RefreshTokenAdminCommand : ICommand<AuthenticationResponse>
{
    public string? RefreshToken { get; set; }
}