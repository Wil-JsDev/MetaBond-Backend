using FluentValidation;
using MetaBond.Application.Interfaces.Repository.Account;

namespace MetaBond.Application.Feature.Chat.Query.GetChatByUserId;

public class GetChatByUserIdQueryValidator : AbstractValidator<GetChatByUserIdQuery>
{
    private readonly IChatRepository _chatRepository;

    public GetChatByUserIdQueryValidator(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;

        RuleFor(query => query.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");

        RuleFor(query => query.ChatId)
            .NotEmpty().WithMessage("Chat ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Chat ID must be a valid GUID.");

        RuleFor(x => x)
            .MustAsync(BeNewMember)
            .WithMessage("The user is already a member of this chat.");
    }

    private async Task<bool> BeNewMember(GetChatByUserIdQuery query, CancellationToken cancellationToken)
    {
        // Check the repository to see if the user is currently in the chat.
        var isAlreadyInChat = await _chatRepository.IsUserInChatAsync(
            query.ChatId,
            query.UserId,
            cancellationToken
        );

        // FluentValidation requires TRUE for success. We return the inverse of the check:
        // If isAlreadyInChat is true, we return false (validation failure).
        // If isAlreadyInChat is false, we return true (validation success).
        return !isAlreadyInChat;
    }
}