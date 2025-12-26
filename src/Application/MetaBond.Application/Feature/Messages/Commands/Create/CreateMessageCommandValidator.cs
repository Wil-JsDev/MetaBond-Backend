using FluentValidation;
using MetaBond.Application.Interfaces.Repository;

namespace MetaBond.Application.Feature.Messages.Commands.Create;

public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    private readonly IMessageRepository _messageRepository;

    public CreateMessageCommandValidator(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;

        RuleFor(cm => cm.Content)
            .NotEmpty()
            .WithMessage("Content is required.");

        RuleFor(m => m.SenderId)
            .MustAsync(async (m, senderId, cancellationToken) =>
            {
                var isMember =
                    await messageRepository.ExistsMessageAsync(
                        chatId: m.ChatId,
                        userId: senderId,
                        cancellationToken: cancellationToken
                    );

                return !isMember;
            })
            .WithMessage("Sender has already sent messages in this chat.");

        RuleFor(cm => cm.ChatId)
            .NotEmpty()
            .WithMessage("ChatId is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("ChatId is invalid.");

        RuleFor(cm => cm.SenderId)
            .NotEmpty()
            .WithMessage("SenderId is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("SenderId is invalid.");
    }
}