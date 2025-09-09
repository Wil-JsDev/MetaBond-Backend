using FluentValidation;

namespace MetaBond.Application.Feature.CommunityMembership.Commands.JoinCommunity;

public class JoinCommunityValidator : AbstractValidator<JoinCommunityCommand>
{
    public JoinCommunityValidator()
    {
        RuleFor(j => j.CommunityId)
            .NotEmpty().WithMessage("The community id is required and cannot be empty or null.");

        RuleFor(j => j.UserId)
            .NotEmpty().WithMessage("The user id is required and cannot be empty or null.");
    }
}