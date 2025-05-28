using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Commands.UpdatePassword;

internal sealed class UpdatePasswordUserCommandHandler(
    IUserRepository userRepository,
    ILogger<UpdatePasswordUserCommandHandler> logger,
    IDistributedCache decoratedCache
    ) : ICommandHandler<UpdatePasswordUserCommand, string>
{
    public async Task<ResultT<string>> Handle(
        UpdatePasswordUserCommand request, 
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}