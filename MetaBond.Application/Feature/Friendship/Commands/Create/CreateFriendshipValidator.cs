using FluentValidation;

namespace MetaBond.Application.Feature.Friendship.Command.Create;

public class CreateFriendshipValidator : AbstractValidator<CreateFriendshipCommand>
{
    public CreateFriendshipValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("The status is required and cannot be empty or null.")
            .IsInEnum().WithMessage("The status must be a valid value.");
    }
}