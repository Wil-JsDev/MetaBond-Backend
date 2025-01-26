using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Communities.Commands.Update
{
    public class UpdateCommunitiesCommand : ICommand<CommunitiesDTos>
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Category { get; set; }
    }
}
