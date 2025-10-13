using FluentValidation;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.CommunityMembership.Commands.UpdateRole;

public class UpdateCommunityMembershipRoleValidator : AbstractValidator<UpdateCommunityMembershipRoleCommand>
{
    public UpdateCommunityMembershipRoleValidator()
    {
        RuleFor(c => c.CommunityId)
            .NotEmpty().WithMessage("The community id is required and cannot be empty.")
            .Must(id => id.HasValue && id.Value != Guid.Empty)
            .WithMessage("The community id must be a valid GUID.");

        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("The user id is required and cannot be empty.")
            .Must(id => id.HasValue && id.Value != Guid.Empty)
            .WithMessage("The user id must be a valid GUID.");

        RuleFor(c => c.Role)
            .NotEmpty().WithMessage("The role is required and cannot be empty.")
            .Must(role => Enum.TryParse(typeof(CommunityMembershipRoles), role, true, out _))
            .WithMessage("The role must be a valid value: Owner, Member, Moderator, or CommunityManager.");
    }
}