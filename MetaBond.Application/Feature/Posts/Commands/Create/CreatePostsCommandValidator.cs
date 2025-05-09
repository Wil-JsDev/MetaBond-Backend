using System.Data;
using FluentValidation;

namespace MetaBond.Application.Feature.Posts.Commands.Create;

public class CreatePostsCommandValidator : AbstractValidator<CreatePostsCommand>
{
    public CreatePostsCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("The title is required and cannot be empty or null.")
            .MaximumLength(50).WithMessage("The title must not exceed 50 characters.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("The content is required and cannot be empty or null.")
            .MaximumLength(150).WithMessage("The content must not exceed 150 characters.");

     
        RuleFor(x => x.CommunitiesId)
            .NotEmpty().WithMessage("The communities id is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The communities ID must be a valid GUID.");

        RuleFor(x => x.CreatedById)
            .NotEmpty().WithMessage("The communities id is required and cannot be empty or null.");
    }
}