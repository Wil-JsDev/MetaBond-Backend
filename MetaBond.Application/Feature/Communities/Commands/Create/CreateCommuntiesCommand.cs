using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Communities.Commands
{
    public sealed class CreateCommuntiesCommand : ICommand<CommunitiesDTos> 
    {
        public string? Name { get; set; }
        
        public string? Description { get; set; }
        public string? Category { get; set; }
    }
}
