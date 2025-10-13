using FluentValidation;

namespace MetaBond.Application.Feature.Chat.Query.GetUserInChat;

public class GetUserByChatIdQueryValidator : AbstractValidator<GetUserByChatIdQuery>
{
    public GetUserByChatIdQueryValidator()
    {
        RuleFor(query => query.ChatId)
            .NotEmpty().WithMessage("Chat ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Chat ID must be a valid GUID.");
    }
}