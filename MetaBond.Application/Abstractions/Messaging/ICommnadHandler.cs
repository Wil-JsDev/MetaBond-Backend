using MediatR;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Abstractions.Messaging
{
    public interface ICommnadHandler<in TCommand> : 
        IRequestHandler<TCommand, Result>
        where TCommand : ICommand;

    public interface ICommandHandler<in TCommand, TResponse> :
        IRequestHandler<TCommand, ResultT<TResponse>> 
        where TCommand : ICommand<TResponse>;

}
