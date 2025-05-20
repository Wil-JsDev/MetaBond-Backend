namespace MetaBond.Application.DTOs.ProgressBoard
{
    public sealed record ProgressBoardDTos
    (
        Guid ProgressBoardId,
        Guid CommunitiesId,
        Guid UserId,
        DateTime? CreatedAt,
        DateTime? UpdatedAt
    );
}
