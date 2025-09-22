using FluentValidation;

namespace MetaBond.Application.Feature.Admin.Commands.Create;

public class CreateAdminCommandValidator : AbstractValidator<CreateAdminCommand>
{
    private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public CreateAdminCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(30).WithMessage("Username must not exceed 30 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(x => x.ImageFile)
            .NotNull().WithMessage("Image file is required.")
            .Must(file => file!.Length > 0).WithMessage("Image file cannot be empty.")
            .Must(file => _allowedExtensions.Contains(Path.GetExtension(file!.FileName).ToLower()))
            .WithMessage("Only .jpg, .jpeg, .png and .webp image formats are allowed.");
    }
}