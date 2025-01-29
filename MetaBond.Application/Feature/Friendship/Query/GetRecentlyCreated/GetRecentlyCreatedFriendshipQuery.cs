using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Friendship.Query.GetRecentlyCreated
{
    public sealed class GetRecentlyCreatedFriendshipQuery : IQuery<IEnumerable<FriendshipDTos>>
    {
        public int Limit { get; set; }
    }
}
