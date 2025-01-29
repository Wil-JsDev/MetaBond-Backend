using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Friendship.Query.GetOrderById
{
    public sealed class GetOrderByIdFriendshipQuery : IQuery<IEnumerable<FriendshipDTos>>
    {
        public string? Sort {  get; set; }
    }
}
