using FluentValidation;

namespace MetaBond.Application.Feature.Posts.Commands.Delete;

public class DeletePostsCommandValidator : AbstractValidator<DeletePostsCommand>
{
    public DeletePostsCommandValidator()
    {
        RuleFor(x => x.PostsId)
            .NotEmpty().WithMessage("The ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The ID must be a valid GUID.");
    }
}