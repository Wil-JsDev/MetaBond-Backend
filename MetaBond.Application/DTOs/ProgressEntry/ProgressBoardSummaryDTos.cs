namespace MetaBond.Application.DTOs.ProgressEntry;

public record ProgressBoardSummaryDTos
(
   Guid ProgressBoardId,
   Guid UserId,
   Guid CommunitiesId,
   DateTime? CreatedAt,
   DateTime? ModifiedAt
);