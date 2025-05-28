using MetaBond.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace MetaBond.Application.Feature.User.Commands.UpdatePhoto;

public sealed class UpdatePhotoUserCommand : ICommand<string>
{
    public Guid UserId { get; set; }
    
    public IFormFile? ImageFile { get; set; }
}