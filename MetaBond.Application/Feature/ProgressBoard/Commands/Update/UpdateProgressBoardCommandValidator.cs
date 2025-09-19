using FluentValidation;

namespace MetaBond.Application.Feature.ProgressBoard.Commands.Update;

public class UpdateProgressBoardCommandValidator : AbstractValidator<UpdateProgressBoardCommand>
{
    public UpdateProgressBoardCommandValidator()
    {
        RuleFor(x => x.ProgressBoardId)
            .NotEmpty().WithMessage("The ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The ID must be a valid GUID.");

        RuleFor(x => x.CommunitiesId)
            .NotEmpty().WithMessage("The community ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The community ID must be a valid GUID.");
    }
}