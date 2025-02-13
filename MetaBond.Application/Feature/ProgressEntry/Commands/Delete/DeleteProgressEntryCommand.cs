
using MetaBond.Application.Abstractions.Messaging;
using System.Windows.Input;

namespace MetaBond.Application.Feature.ProgressEntry.Commands.Delete
{
    public sealed class DeleteProgressEntryCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
    }
}
