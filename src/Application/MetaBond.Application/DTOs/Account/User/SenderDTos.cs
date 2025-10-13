namespace MetaBond.Application.DTOs.Account.User;

public sealed record SenderDTos(
    Guid SenderId,
    string? DisplayName,
    string? Photo
);