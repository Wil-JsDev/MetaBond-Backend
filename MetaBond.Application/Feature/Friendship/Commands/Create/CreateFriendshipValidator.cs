using FluentValidation;

namespace MetaBond.Application.Feature.Friendship.Commands.Create;

public class CreateFriendshipValidator : AbstractValidator<CreateFriendshipCommand>
{
    public CreateFriendshipValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("The status is required and cannot be empty or null.")
            .IsInEnum().WithMessage("The status must be a valid value.");

        RuleFor(x => x.RequesterId)
            .NotEmpty().WithMessage("The requesterId is required and cannot be empty or null.");
        
        RuleFor(x => x.AddresseeId)
            .NotEmpty().WithMessage("The addresseeId is required and cannot be empty or null.");
    }
}