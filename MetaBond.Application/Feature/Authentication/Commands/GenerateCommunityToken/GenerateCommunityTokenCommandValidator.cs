using FluentValidation;

namespace MetaBond.Application.Feature.Authentication.Commands.GenerateCommunityToken;

public class GenerateCommunityTokenCommandValidator : AbstractValidator<GenerateCommunityTokenCommand>
{
    public GenerateCommunityTokenCommandValidator()
    {
        RuleFor(cm => cm.CommunityId)
            .NotEmpty().WithMessage("CommunityId is required.");
        
        RuleFor(cm => cm.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }
}