using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class ProgressEntryMapper
{
    public static ProgressEntryDTos ToDto(ProgressEntry progressEntry)
    {
        return new ProgressEntryDTos
        (
            ProgressEntryId: progressEntry.Id,
            ProgressBoardId: progressEntry.ProgressBoardId,
            UserId: progressEntry.UserId,
            Description: progressEntry.Description,
            CreatedAt: progressEntry.CreatedAt,
            UpdateAt: progressEntry.UpdateAt
        );
    }

    public static IEnumerable<ProgressEntryWithProgressBoardDTos> ToProgressEntryWithBoardDtos(
        this IEnumerable<ProgressEntry> entries)
    {
        return entries.Select(e => e.ToProgressEntryWithBoardDto());
    }

    public static IEnumerable<ProgressEntryBasicDTos> ToBasicDtos(this IEnumerable<ProgressEntry> entries)
    {
        return entries.Select(e => e.ToBasicDto());
    }

    public static IEnumerable<ProgressEntriesWithUserDTos> ProgressEntriesWithUserToListDto(
        this IEnumerable<ProgressEntry> entries)
    {
        return entries.Select(e => e.ProgressEntriesWithUserToDto());
    }

    #region Private methods

    private static ProgressEntriesWithUserDTos ProgressEntriesWithUserToDto(this ProgressEntry entry)
    {
        return new ProgressEntriesWithUserDTos(
            ProgressEntryId: entry.Id,
            ProgressBoardId: entry.ProgressBoardId,
            User: new UserProgressEntryDTos(
                UserId: entry.User!.Id,
                Username: entry.User.Username,
                Photo: entry.User.Photo
            ),
            Description: entry.Description,
            CreatedAt: entry.CreatedAt,
            UpdateAt: entry.UpdateAt
        );
    }

    private static ProgressEntryBasicDTos ToBasicDto(this ProgressEntry entry)
    {
        return new ProgressEntryBasicDTos(
            ProgressEntryId: entry.Id,
            Description: entry.Description,
            ProgressBoardId: entry.ProgressBoardId,
            UserId: entry.UserId
        );
    }

    private static ProgressBoardSummaryDTos? ToProgressBoardSummaryDto(this ProgressBoard board, Guid userId)
    {
        return new ProgressBoardSummaryDTos(
            ProgressBoardId: board.Id,
            UserId: userId,
            CommunitiesId: board.CommunitiesId,
            CreatedAt: board.CreatedAt,
            ModifiedAt: board.UpdatedAt
        );
    }

    private static ProgressEntryWithProgressBoardDTos ToProgressEntryWithBoardDto(this ProgressEntry entry)
    {
        return new ProgressEntryWithProgressBoardDTos(
            ProgressEntryId: entry.Id,
            UserId: entry.UserId,
            ProgressBoard: entry.ProgressBoard != null
                ? entry.ProgressBoard.ToProgressBoardSummaryDto(entry.UserId)
                : null,
            Description: entry.Description,
            CreatedAt: entry.CreatedAt,
            UpdateAt: entry.UpdateAt
        );
    }

    #endregion
}