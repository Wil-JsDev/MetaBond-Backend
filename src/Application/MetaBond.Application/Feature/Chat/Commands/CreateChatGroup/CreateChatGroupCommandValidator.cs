using FluentValidation;

namespace MetaBond.Application.Feature.Chat.Commands.CreateChatGroup;

public class CreateChatGroupCommandValidator : AbstractValidator<CreateChatGroupCommand>
{
    private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public CreateChatGroupCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");

        RuleFor(command => command.CommunityId)
            .NotEmpty().WithMessage("Community ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Community ID must be a valid GUID.");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Group name is required.")
            .NotNull().WithMessage("Group name cannot be null.")
            .MaximumLength(50).WithMessage("Group name cannot exceed 50 characters.");

        RuleFor(x => x.Photo)
            .NotNull().WithMessage("Image file is required.")
            .Must(file => file!.Length > 0).WithMessage("Image file cannot be empty.")
            .Must(file => _allowedExtensions.Contains(Path.GetExtension(file!.FileName).ToLower()))
            .WithMessage("Only .jpg, .jpeg, .png and .webp image formats are allowed.");
    }
}