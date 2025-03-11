using FluentValidation;

namespace MetaBond.Application.Feature.ProgressEntry.Commands.Delete;

public class DeleteProgressEntryCommandValidator : AbstractValidator<DeleteProgressEntryCommand>
{
    public DeleteProgressEntryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("The ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The ID must be a valid GUID.");

    }
}