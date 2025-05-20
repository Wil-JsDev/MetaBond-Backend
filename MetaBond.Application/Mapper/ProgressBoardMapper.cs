using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.DTOs.ProgressEntry;

namespace MetaBond.Application.Mapper;
public static class ProgressBoardMapper
{
    public static ProgressBoardWithUserDTos ToDTo(Domain.Models.ProgressBoard progressBoard)
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
                : new CommunitiesDTos(
                    CommunitiesId: progressBoard.Communities.Id,
                    Name: progressBoard.Communities.Name,
                    Category: progressBoard.Communities.Category,
                    CreatedAt: progressBoard.Communities.CreateAt
                ),
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
}