namespace MetaBond.Application.DTOs.ProgressEntry;

public record ProgressBoardSummaryDTos
(
   Guid ProgressBoardId,
   Guid CommunitiesId,
   DateTime? CreatedAt,
   DateTime? ModifiedAt
);