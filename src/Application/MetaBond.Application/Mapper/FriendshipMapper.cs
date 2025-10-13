using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class FriendshipMapper
{
    public static FriendshipDTos MapFriendshipDTos(Friendship friendship)
    {
        return new FriendshipDTos
        (
            FriendshipId: friendship.Id,
            Status: friendship.Status,
            RequesterId: friendship.RequesterId,
            AddresseeId: friendship.AddresseeId,
            CreatedAt: friendship.CreateAdt
        );
    }

    public static IEnumerable<FriendshipWithUserDTos> ToFriendshipWithUserDtos(
        this IEnumerable<Friendship> friendships)
    {
        return friendships.Select(x => new FriendshipWithUserDTos(
            FriendshipId: x.Id,
            Status: x.Status,
            RequesterId: x.RequesterId,
            AddresseeId: x.AddresseeId,
            User: x.Requester != null
                ? new UserFriendshipDTos(
                    UserId: x.Requester.Id,
                    FirstName: x.Requester.FirstName,
                    LastName: x.Requester.LastName,
                    Username: x.Requester.Username,
                    Photo: x.Requester.Photo
                )
                : null,
            CreatedAt: x.CreateAdt
        ));
    }

    public static UserWithFriendshipDTos MapUserWithFriendshipDTos(User user)
    {
        var requesterFriendships = user.ReceivedFriendRequests?
            .Select(x => new RequesterFriendshipDTos(
                FriendshipId: x.Id,
                RequesterId: x.RequesterId ?? Guid.Empty,
                Username: x.Requester?.Username ?? string.Empty,
                StatusFriendship: x.Status
            ))
            .ToList() ?? new List<RequesterFriendshipDTos>();

        var addresseeFriendships = user.SentFriendRequests?
            .Select(x => new AddresseeFriendshipDTos(
                FriendshipId: x.Id,
                AddresseeId: x.AddresseeId ?? Guid.Empty,
                Username: x.Addressee?.Username ?? string.Empty,
                StatusFriendship: x.Status
            ))
            .ToList() ?? new List<AddresseeFriendshipDTos>();

        return new UserWithFriendshipDTos(
            UserId: user.Id,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Photo: user.Photo,
            Requester: requesterFriendships,
            Addressee: addresseeFriendships
        );
    }
}