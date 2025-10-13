using FluentValidation;

namespace MetaBond.Application.Feature.Communities.Commands.Create;

public class CreateCommunitiesCommandValidator : AbstractValidator<CreateCommunitiesCommand>
{
    private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public CreateCommunitiesCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The name is required and cannot be empty or null.")
            .MaximumLength(50).WithMessage("The name must not exceed 50 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("The description is required and cannot be empty or null.")
            .MaximumLength(255).WithMessage("The description must not exceed 255 characters.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("The category is required and cannot be empty or null.");

        RuleFor(x => x.ImageFile)
            .NotNull().WithMessage("Image file is required.")
            .Must(file => file!.Length > 0).WithMessage("Image file cannot be empty.")
            .Must(file => _allowedExtensions.Contains(Path.GetExtension(file!.FileName).ToLower()))
            .WithMessage("Only .jpg, .jpeg, .png and .webp image formats are allowed.");
    }
}