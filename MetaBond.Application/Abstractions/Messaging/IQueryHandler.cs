using MediatR;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Abstractions.Messaging;

public interface IQueryHandler<in TQuery, TResponse> :
    IRequestHandler<TQuery, ResultT<TResponse>>
    where TQuery : IQuery<TResponse>;