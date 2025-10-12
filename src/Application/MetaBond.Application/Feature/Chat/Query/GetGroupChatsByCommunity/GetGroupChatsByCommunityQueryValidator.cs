using FluentValidation;

namespace MetaBond.Application.Feature.Chat.Query.GetGroupChatsByCommunity;

public class GetGroupChatsByCommunityQueryValidator : AbstractValidator<GetGroupChatsByCommunityQuery>
{
    public GetGroupChatsByCommunityQueryValidator()
    {
        RuleFor(query => query.CommunityId)
            .NotEmpty().WithMessage("Community ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Community ID must be a valid GUID.");
    }
}