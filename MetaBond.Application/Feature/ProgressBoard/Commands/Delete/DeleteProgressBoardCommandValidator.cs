using FluentValidation;

namespace MetaBond.Application.Feature.ProgressBoard.Commands.Delete;

public class DeleteProgressBoardCommandValidator : AbstractValidator<DeleteProgressBoardCommand>
{
    public DeleteProgressBoardCommandValidator()
    {
        RuleFor(x => x.ProgressBoardId)
            .NotEmpty().WithMessage("The ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The ID must be a valid GUID.");

    }
}