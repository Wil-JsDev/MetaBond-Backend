using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Communities.Querys.Filter
{
    public sealed class FilterCommunitiesQuery : IQuery<IEnumerable<CommunitiesDTos>>
    {
        public string? Category {  get; set; }       
    }
}
