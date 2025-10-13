using FluentValidation;

namespace MetaBond.Application.Feature.ParticipationInEvent.Commands.Create;

public class CreateParticipationInEventCommandValidator : AbstractValidator<CreateParticipationInEventCommand>
{
    public CreateParticipationInEventCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("The event ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The event ID must be a valid GUID.");
    }
}