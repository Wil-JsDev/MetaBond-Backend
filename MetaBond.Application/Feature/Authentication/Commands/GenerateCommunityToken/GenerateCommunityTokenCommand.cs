using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Auth;

namespace MetaBond.Application.Feature.Authentication.Commands.GenerateCommunityToken;

public sealed class GenerateCommunityTokenCommand : ICommand<AuthenticationCommunityResponse>
{
    public Guid CommunityId { get; set; }
    
    public Guid UserId { get; set; }
}