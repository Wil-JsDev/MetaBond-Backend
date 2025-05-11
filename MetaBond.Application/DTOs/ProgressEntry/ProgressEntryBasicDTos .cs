
namespace MetaBond.Application.DTOs.ProgressEntry
{
    public sealed record ProgressEntryBasicDTos(Guid ProgressEntryId, string? Description, Guid ProgressBoardId,Guid UserId);
}
