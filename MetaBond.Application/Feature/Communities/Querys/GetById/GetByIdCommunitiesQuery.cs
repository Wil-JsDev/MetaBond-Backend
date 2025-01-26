using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Communities.Querys.GetById
{
    public sealed class GetByIdCommunitiesQuery : IQuery<CommunitiesDTos>
    {
        public Guid Id { get; set; }
    }
}
