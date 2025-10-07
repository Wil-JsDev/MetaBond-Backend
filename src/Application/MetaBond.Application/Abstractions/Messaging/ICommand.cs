using MediatR;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<ResultT<TResponse>>, IBaseCommand;

public interface IBaseCommand;