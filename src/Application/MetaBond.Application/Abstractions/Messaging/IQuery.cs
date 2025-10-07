using MediatR;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<ResultT<TResponse>>;