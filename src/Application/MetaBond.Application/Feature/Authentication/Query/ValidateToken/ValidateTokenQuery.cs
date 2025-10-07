using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Auth;

namespace MetaBond.Application.Feature.Authentication.Query.ValidateToken;

public sealed class ValidateTokenQuery : IQuery<JwtUserData>;