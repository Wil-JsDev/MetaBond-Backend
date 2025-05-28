using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;

namespace MetaBond.Application.Feature.User.Query.GetByUsername;

public sealed class GetByUsernameUserQuery : IQuery<UserDTos>
{
    public string? Username { get; set; }
}