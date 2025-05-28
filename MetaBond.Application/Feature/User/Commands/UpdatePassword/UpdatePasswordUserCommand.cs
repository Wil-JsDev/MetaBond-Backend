using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.User.Commands.UpdatePassword;

public sealed class UpdatePasswordUserCommand : ICommand<string>
{
    public Guid UserId { get; set; }
    public string? NewPassword { get; set; }
    
    public string? ConfirmNewPassword { get; set; }
    
    public string? Token { get; set; }
}