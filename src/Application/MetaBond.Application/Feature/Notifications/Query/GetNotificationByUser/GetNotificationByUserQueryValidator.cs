using FluentValidation;

namespace MetaBond.Application.Feature.Notifications.Query.GetNotificationByUser;

public class GetNotificationByUserQueryValidator : AbstractValidator<GetNotificationByUserQuery>
{
    public GetNotificationByUserQueryValidator()
    {
        RuleFor(nt => nt.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .NotEqual(Guid.Empty).WithMessage("UserId must be a valid GUID.");

        RuleFor(nt => nt.NotificationId)
            .NotEmpty().WithMessage("NotificationId is required.")
            .NotEqual(Guid.Empty).WithMessage("NotificationId must be a valid GUID.");
    }
}