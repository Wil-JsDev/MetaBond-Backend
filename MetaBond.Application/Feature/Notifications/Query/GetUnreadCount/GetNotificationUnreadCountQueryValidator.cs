using FluentValidation;

namespace MetaBond.Application.Feature.Notifications.Query.GetUnreadCount;

public class GetNotificationUnreadCountQueryValidator : AbstractValidator<GetNotificationUnreadCountQuery>
{
    public GetNotificationUnreadCountQueryValidator()
    {
        RuleFor(nt => nt.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .NotEqual(Guid.Empty).WithMessage("UserId must be a valid GUID.");
    }
}