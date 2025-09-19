using FluentValidation;

namespace MetaBond.Application.Feature.User.Commands.UpdatePhoto;

public sealed class UpdatePhotoUserCommandValidator : AbstractValidator<UpdatePhotoUserCommand>
{
    private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public UpdatePhotoUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.ImageFile)
            .NotNull().WithMessage("Image file is required.")
            .Must(file => file!.Length > 0).WithMessage("Image file cannot be empty.")
            .Must(file => _allowedExtensions.Contains(Path.GetExtension(file!.FileName).ToLower()))
            .WithMessage("Only .jpg, .jpeg, .png and .webp image formats are allowed.");
    }
}