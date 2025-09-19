using FluentValidation;

namespace MetaBond.Application.Feature.Events.Commands.Update;

public class UpdateEventsCommandValidator : AbstractValidator<UpdateEventsCommand>
{
    public UpdateEventsCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("The ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The ID must be a valid GUID.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("The title is required and cannot be empty or null.")
            .MaximumLength(50).WithMessage("The title must not exceed 50 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("The description is required and cannot be empty or null.")
            .MaximumLength(255).WithMessage("The description must not exceed 255 characters.");
    }
}