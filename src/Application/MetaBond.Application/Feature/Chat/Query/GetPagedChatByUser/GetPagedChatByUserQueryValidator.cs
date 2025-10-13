using FluentValidation;

namespace MetaBond.Application.Feature.Chat.Query.GetPagedChatByUser;

public class GetPagedChatByUserQueryValidator : AbstractValidator<GetPagedChatByUserQuery>
{
    public GetPagedChatByUserQueryValidator()
    {
        RuleFor(query => query.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");
    }
}