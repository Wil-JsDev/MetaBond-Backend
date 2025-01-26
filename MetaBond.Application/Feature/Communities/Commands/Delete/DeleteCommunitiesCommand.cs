using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MetaBond.Application.Feature.Communities.Commands.Delete
{
    public class DeleteCommunitiesCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
    }
}
