using FluentValidation;

namespace MetaBond.Application.Feature.Events.Commands.Delete;

public class DeleteEventsCommandValidator : AbstractValidator<DeleteEventsCommand>
{
    public DeleteEventsCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("The ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The ID must be a valid GUID.");
    }
}