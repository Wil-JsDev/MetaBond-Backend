using FluentValidation;

namespace MetaBond.Application.Feature.Chat.Commands.CreateChatPrivate;

public sealed class CreateChatPrivateCommandValidator : AbstractValidator<CreateChatPrivateCommand>
{
    public CreateChatPrivateCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");
    }
}