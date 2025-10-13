namespace MetaBond.Application.DTOs.Account.Roles;

public sealed record RolesDto(
    Guid? RolesId,
    string? NameRole,
    string? Description
);