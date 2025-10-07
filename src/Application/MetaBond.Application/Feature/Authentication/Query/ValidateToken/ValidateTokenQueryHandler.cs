using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Auth;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Feature.Authentication.Query.ValidateToken;

internal sealed class ValidateTokenQueryHandler(
    ICurrentService currentService
) : IQueryHandler<ValidateTokenQuery, JwtUserData>
{
    public Task<ResultT<JwtUserData>> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
    {
        var response = new JwtUserData(
            message: JwtMessages.TokenValidated,
            data: new
            {
                Id = currentService.CurrentId,
                UserName = currentService.UserName,
                Roles = currentService.GetRoles.ToList()
            }
        );

        return Task.FromResult(ResultT<JwtUserData>.Success(response));
    }
}