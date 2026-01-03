using FluentValidation;

namespace MetaBond.Application.Feature.Messages.Commands.MarkAsRead;

public sealed class MarkMessagesAsReadCommandValidator : AbstractValidator<MarkMessagesAsReadCommand>
{
    public MarkMessagesAsReadCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("ChatId must not be empty.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId must not be empty.");
    }
}