namespace MetaBond.Application.DTOs.ProgressEntry;

public sealed record ProgressBoardSummaryDTos
(
   Guid ProgressBoardId,
   Guid UserId,
   Guid CommunitiesId,
   DateTime? CreatedAt,
   DateTime? ModifiedAt
);