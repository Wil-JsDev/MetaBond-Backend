using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.User.Commands.UpdatePassword;

public sealed class UpdatePasswordUserCommand : ICommand<string>
{
    public string? Email { get; set; }
    
    public string? NewPassword { get; set; }
    
    public string? ConfirmNewPassword { get; set; }
    
    public string? Code { get; set; }
}