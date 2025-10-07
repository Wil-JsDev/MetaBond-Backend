using FluentValidation;

namespace MetaBond.Application.Feature.Notifications.Query.GetById;

public class GetByIdNotificationQueryValidator : AbstractValidator<GetByIdNotificationQuery>
{
    public GetByIdNotificationQueryValidator()
    {
        RuleFor(nt => nt.NotificationId)
            .NotEmpty().WithMessage("NotificationId is required.")
            .NotEqual(Guid.Empty).WithMessage("NotificationId must be a valid GUID.");
    }
}