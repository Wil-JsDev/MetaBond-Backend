namespace MetaBond.Application.DTOs.ProgressBoard
{
    public sealed record ProgressBoardDTos
    (
        Guid ProgressBoardId,
        Guid CommunitiesId,
        DateTime? CreatedAt,
        DateTime? UpdatedAt
    );
}
