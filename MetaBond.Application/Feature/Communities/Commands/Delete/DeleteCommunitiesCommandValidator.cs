using FluentValidation;

namespace MetaBond.Application.Feature.Communities.Commands.Delete;

public class DeleteCommunitiesCommandValidator : AbstractValidator<DeleteCommunitiesCommand>
{
    public DeleteCommunitiesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("The ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The ID must be a valid GUID.");
    }
}