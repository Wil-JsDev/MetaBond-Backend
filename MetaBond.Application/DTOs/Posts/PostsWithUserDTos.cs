using MetaBond.Application.DTOs.Account.User;

namespace MetaBond.Application.DTOs.Posts;

public record PostsWithUserDTos
(
    Guid PostsId,
    string? Title,
    string? Content,
    string? ImageUrl,
    UserPostsDTos? CreatedBy,
    Guid? CommunitiesId
);