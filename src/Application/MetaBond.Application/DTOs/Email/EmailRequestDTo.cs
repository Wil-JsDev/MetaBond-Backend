namespace MetaBond.Application.DTOs.Email;

public sealed record EmailRequestDTo(string? To, string? Body, string? Subject);