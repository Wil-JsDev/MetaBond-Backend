using FluentValidation;

namespace MetaBond.Application.Feature.Notifications.Query.GetNotificationRecentByUser;

public class GetNotificationRecentByUserQueryValidator : AbstractValidator<GetNotificationRecentByUserQuery>
{
    public GetNotificationRecentByUserQueryValidator()
    {
        RuleFor(nt => nt.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .NotEqual(Guid.Empty).WithMessage("UserId must be a valid GUID.");

        RuleFor(nt => nt.Take)
            .GreaterThan(0).WithMessage("Take must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Take cannot exceed 100.");
    }
}