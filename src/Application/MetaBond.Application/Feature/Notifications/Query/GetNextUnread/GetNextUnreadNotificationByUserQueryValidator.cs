using FluentValidation;

namespace MetaBond.Application.Feature.Notifications.Query.GetNextUnread;

public class GetNextUnreadNotificationByUserQueryValidator : AbstractValidator<GetNextUnreadNotificationByUserQuery>
{
    public GetNextUnreadNotificationByUserQueryValidator()
    {
        RuleFor(nt => nt.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .NotEqual(Guid.Empty).WithMessage("UserId must be a valid GUID.");
    }
}