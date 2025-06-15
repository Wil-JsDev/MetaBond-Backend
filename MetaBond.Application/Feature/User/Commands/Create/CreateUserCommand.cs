using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using Microsoft.AspNetCore.Http;

namespace MetaBond.Application.Feature.User.Commands.Create;

public sealed class CreateUserCommand : ICommand<UserDTos>
{
    public string? FirstName { get; set; }
    
    public string? LastName {get; set;}
    
    public string? Username { get; set; }
    
    public string? Email { get; set; }
    
    public IFormFile? ImageFile { get; set; }
    
    public string? Password { get; set; }
}