using FluentValidation;

namespace MetaBond.Application.Feature.ProgressEntry.Commands.Create;

public class CreateProgressEntryCommandValidator : AbstractValidator<CreateProgressEntryCommand>
{
    public CreateProgressEntryCommandValidator()
    {
        RuleFor(x => x.ProgressBoardId)
            .NotEmpty().WithMessage("The ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The ID must be a valid GUID.");
        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("The description is required and cannot be empty or null.")
            .MaximumLength(250).WithMessage("The description must not exceed 250 characters.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("The user ID is required and cannot be empty or null.");
    }
}