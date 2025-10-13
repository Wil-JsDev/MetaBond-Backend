using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class ProgressBoardMapper
{
    public static ProgressBoardDTos ProgressBoardToDto(ProgressBoard progressBoard)
    {
        return new ProgressBoardDTos
        (
            ProgressBoardId: progressBoard.Id,
            CommunitiesId: progressBoard.CommunitiesId,
            UserId: progressBoard.UserId,
            CreatedAt: progressBoard.CreatedAt,
            UpdatedAt: progressBoard.UpdatedAt
        );
    }

    public static ProgressBoardWithUserDTos ToDTo(ProgressBoard progressBoard)
    {
        var firstEntry = progressBoard.ProgressEntries?.FirstOrDefault();

        return new ProgressBoardWithUserDTos
        (
            ProgressBoardId: progressBoard.Id,
            ProgressEntry: firstEntry == null
                ? null
                : new ProgressEntryWithBoardDTos(
                    ProgressEntryId: firstEntry.Id,
                    Description: firstEntry.Description,
                    UserId: firstEntry.User?.Id ?? Guid.Empty
                ),
            Communities: progressBoard.Communities == null
                ? null!
                : CommunityMapper.MapCommunitiesDTos(progressBoard.Communities),
            User: progressBoard.User == null
                ? null!
                : new UserProgressEntryDTos(
                    UserId: progressBoard.User.Id,
                    Username: progressBoard.User.Username,
                    Photo: progressBoard.User.Photo
                ),
            CreatedAt: progressBoard.CreatedAt
        );
    }

    public static ProgressBoardWithProgressEntryDTos ToProgressBoardWithEntriesDto(
        this ProgressBoard board,
        IEnumerable<ProgressEntry> progressEntries)
    {
        return new ProgressBoardWithProgressEntryDTos(
            ProgressBoardId: board.Id,
            CommunitiesId: board.CommunitiesId,
            UserId: board.UserId,
            ProgressEntries: progressEntries.Select(e => e.ToProgressEntrySummaryDto()).ToList(),
            CreatedAt: board.CreatedAt,
            UpdatedAt: board.UpdatedAt
        );
    }

    #region private methods

    private static ProgressEntrySummaryDTos ToProgressEntrySummaryDto(this ProgressEntry entry)
    {
        return new ProgressEntrySummaryDTos(
            ProgressEntryId: entry.Id,
            Description: entry.Description,
            UserId: entry.UserId,
            CreatedAt: entry.CreatedAt,
            ModifiedAt: entry.UpdateAt
        );
    }

    #endregion
}