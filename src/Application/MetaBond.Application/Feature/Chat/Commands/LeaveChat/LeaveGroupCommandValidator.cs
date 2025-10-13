using FluentValidation;
using MetaBond.Application.Interfaces.Repository.Account;

namespace MetaBond.Application.Feature.Chat.Commands.LeaveChat;

public class LeaveGroupCommandValidator : AbstractValidator<LeaveGroupCommand>
{
    private readonly IChatRepository _chatRepository;

    public LeaveGroupCommandValidator(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");

        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Chat ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Chat ID must be a valid GUID.");

        // Business Rule: The user MUST be a current member to leave the chat.
        RuleFor(x => x)
            .MustAsync(BeCurrentMember)
            .WithMessage("The user is not a member of this chat.");
    }

    private async Task<bool> BeCurrentMember(LeaveGroupCommand command, CancellationToken cancellationToken)
    {
        return await _chatRepository.IsUserInChatAsync(
            command.ChatId,
            command.UserId,
            cancellationToken
        );
    }
}