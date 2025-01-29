using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedBefore
{
    public sealed class GetCreatedBeforeFriendshipQuery : IQuery<IEnumerable<FriendshipDTos>>
    {
        public PastDateRangeType PastDateRangeType {  get; set; }
    }
}
