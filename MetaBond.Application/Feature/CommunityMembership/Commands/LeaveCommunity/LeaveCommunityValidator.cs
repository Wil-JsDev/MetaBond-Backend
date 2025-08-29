using FluentValidation;

namespace MetaBond.Application.Feature.CommunityMembership.Commands.LeaveCommunity;

public class LeaveCommunityValidator : AbstractValidator<LeaveCommunityCommand>
{
    public LeaveCommunityValidator()
    {
        RuleFor(x => x.CommunityId)
            .NotNull().WithMessage("CommunityId is required.")
            .NotEqual(Guid.Empty).WithMessage("CommunityId must be a valid GUID.");

        RuleFor(x => x.UserId)
            .NotNull().WithMessage("UserId is required.")
            .NotEqual(Guid.Empty).WithMessage("UserId must be a valid GUID.");
    }
}