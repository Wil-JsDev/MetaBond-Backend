using FluentValidation;

namespace MetaBond.Application.Feature.Events.Commands.Create;

public class CreateEventsCommandValidator : AbstractValidator<CreateEventsCommand>
{
    public CreateEventsCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("The title is required and cannot be empty or null.")
            .MaximumLength(50).WithMessage("The title must not exceed 50 characters.");
        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("The description is required and cannot be empty or null.")
            .MaximumLength(255).WithMessage("The description must not exceed 255 characters.");

        RuleFor(x => x.CommunitiesId)
            .NotEmpty().WithMessage("The communities id is required and cannot be empty or null.");

        RuleFor(x => x.DateAndTime)
            .NotEmpty().WithMessage("The date and time are required and cannot be empty or null.");
    }
}