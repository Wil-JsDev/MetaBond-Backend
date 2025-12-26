using FluentValidation;

namespace MetaBond.Application.Feature.Messages.Commands.UpdateEdited;

public class UpdateEditedMessageCommandValidator : AbstractValidator<UpdateEditedMessageCommand>
{
    public UpdateEditedMessageCommandValidator()
    {
        RuleFor(ms => ms.Content)
            .NotEmpty()
            .WithMessage("Content is required.");

        RuleFor(ms => ms.MessageId)
            .NotEmpty()
            .WithMessage("MessageId is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("MessageId is invalid.");
    }
}