using FluentValidation;

namespace MetaBond.Application.Feature.ProgressBoard.Commands.Create;

public class CreateProgressBoardCommandValidator : AbstractValidator<CreateProgressBoardCommand>
{
    public CreateProgressBoardCommandValidator()
    {
        RuleFor(x => x.CommunitiesId)
            .NotEmpty().WithMessage("The community ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The community ID must be a valid GUID.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("The user ID is required and cannot be empty or null.");
    }
}