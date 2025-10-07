using FluentValidation;

namespace MetaBond.Application.Feature.Notifications.Query.GetPagedUserId;

public class GetNotificationsByUserIdPagedQueryValidator : AbstractValidator<GetNotificationsByUserIdPagedQuery>
{
    private const int MaxPageSize = 100; // limit

    public GetNotificationsByUserIdPagedQueryValidator()
    {
        RuleFor(us => us.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .NotEqual(Guid.Empty).WithMessage("UserId must be a valid GUID.");

        RuleFor(us => us.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0.");

        RuleFor(us => us.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0.")
            .LessThanOrEqualTo(MaxPageSize)
            .WithMessage($"PageSize cannot exceed {MaxPageSize}.");
    }
}