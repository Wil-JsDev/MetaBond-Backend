using FluentValidation;

namespace MetaBond.Application.Feature.Friendship.Command.Update;

public class UpdateFriendshipCommandValidator : AbstractValidator<UpdateFriendshipCommand>
{
    public UpdateFriendshipCommandValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("The status is required and cannot be empty or null.")
            .IsInEnum().WithMessage("The status must be a valid value.");
    }
}