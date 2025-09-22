using FluentValidation;

namespace MetaBond.Application.Feature.Admin.Query.GetPagedUserStatus;

public class GetPagedUserStatusQueryValidator : AbstractValidator<GetPagedUserStatusQuery>
{
    public GetPagedUserStatusQueryValidator()
    {
        RuleFor(x => x.StatusAccount)
            .IsInEnum()
            .WithMessage("Invalid status account value.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}