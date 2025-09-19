using FluentValidation;

namespace MetaBond.Application.Feature.Friendship.Commands.Delete;

public class DeleteFriendshipCommandValidator : AbstractValidator<DeleteFriendshipCommand>
{
    public DeleteFriendshipCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("The ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The ID must be a valid GUID.");
    }
}