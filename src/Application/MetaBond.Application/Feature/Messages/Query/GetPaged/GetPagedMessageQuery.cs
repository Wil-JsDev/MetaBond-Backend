using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Message;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Messages.Query.GetPaged;

public sealed class GetPagedMessageQuery : IQuery<PagedResult<MessageWithUserDTos>>
{
    public Guid ChatId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}