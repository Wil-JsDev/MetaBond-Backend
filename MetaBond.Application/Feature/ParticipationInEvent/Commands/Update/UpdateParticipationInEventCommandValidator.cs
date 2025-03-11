using FluentValidation;

namespace MetaBond.Application.Feature.ParticipationInEvent.Commands.Update;

public class UpdateParticipationInEventCommandValidator : AbstractValidator<UpdateParticipationInEventCommand>
{
    public UpdateParticipationInEventCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("The ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The ID must be a valid GUID.");
        
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("The event ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The event ID must be a valid GUID.");
    }
}