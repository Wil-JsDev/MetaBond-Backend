using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Admin;
using Microsoft.AspNetCore.Http;

namespace MetaBond.Application.Feature.Admin.Commands.Create;

public sealed class CreateAdminCommand : ICommand<AdminDto>
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public IFormFile? ImageFile { get; set; }

    public string? Password { get; set; }
}