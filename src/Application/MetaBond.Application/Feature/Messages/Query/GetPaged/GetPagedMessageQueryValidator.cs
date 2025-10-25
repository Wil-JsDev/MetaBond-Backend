using FluentValidation;

namespace MetaBond.Application.Feature.Messages.Query.GetPaged;

public class GetPagedMessageQueryValidator : AbstractValidator<GetPagedMessageQuery>
{
    public GetPagedMessageQueryValidator()
    {
        RuleFor(ms => ms.ChatId)
            .NotEmpty().WithMessage("ChatId is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("ChatId is invalid.");
    }
}