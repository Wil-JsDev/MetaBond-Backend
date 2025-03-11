using FluentValidation;

namespace MetaBond.Application.Feature.ProgressEntry.Commands.Update;

public class UpdateProgressEntryCommandValidator : AbstractValidator<UpdateProgressEntryCommand>
{
    public UpdateProgressEntryCommandValidator()
    {
        RuleFor(x => x.ProgressEntryId)
            .NotEmpty().WithMessage("The ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The ID must be a valid GUID.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("The description is required and cannot be empty or null.")
            .MaximumLength(250).WithMessage("The description must not exceed 250 characters.");
    }
}