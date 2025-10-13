using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;

namespace MetaBond.Application.Feature.User.Commands.Update;

public class UpdateUserCommand : ICommand<UpdateUserDTos>
{
    public Guid UserId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }
}