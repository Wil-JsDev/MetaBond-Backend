using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.User.Commands.ConfirmAccount;

public class ConfirmAccountCommand : ICommand<string>
{
 
    public Guid? UserId  { get; set; }
    public string? Code { get; set; }
}