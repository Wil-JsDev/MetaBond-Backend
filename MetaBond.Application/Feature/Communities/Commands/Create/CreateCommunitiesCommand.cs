using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using Microsoft.AspNetCore.Http;

namespace MetaBond.Application.Feature.Communities.Commands.Create;

public sealed class CreateCommunitiesCommand : ICommand<CommunitiesDTos>
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public IFormFile? ImageFile { get; set; }

    public Guid? CategoryId { get; set; }
}