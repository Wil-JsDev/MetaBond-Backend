using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;

namespace MetaBond.Application.Feature.Communities.Query.Filter;

    public sealed class FilterCommunitiesQuery : IQuery<IEnumerable<CommunitiesDTos>>
    {
        public string? Category {  get; set; }       
    }

