using FluentValidation;

namespace MetaBond.Application.Feature.Chat.Query.GetChatWithLastMessage;

public class GetChatWithLastMessageQueryValidator : AbstractValidator<GetChatWithLastMessageQuery>
{
    public GetChatWithLastMessageQueryValidator()
    {
        RuleFor(query => query.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");
    }
}