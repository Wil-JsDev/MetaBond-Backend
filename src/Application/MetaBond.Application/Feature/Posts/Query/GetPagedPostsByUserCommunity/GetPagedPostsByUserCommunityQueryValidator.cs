using FluentValidation;

namespace MetaBond.Application.Feature.Posts.Query.GetPagedPostsByUserCommunity;

public class GetPagedPostsByUserCommunityQueryValidator : AbstractValidator<GetPagedPostsByUserCommunityQuery>
{
    public GetPagedPostsByUserCommunityQueryValidator()
    {
        RuleFor(x => x.CommunitiesId)
            .NotEmpty()
            .WithMessage("community ID is required.");

        RuleFor(x => x.CreatedById)
            .NotEmpty()
            .WithMessage("user ID is required.");
    }
}