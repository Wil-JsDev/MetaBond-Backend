﻿using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Communities.Query.GetPostsAndEvents;

    public sealed class GetCommunityDetailsByIdQuery : IQuery<IEnumerable<PostsAndEventsDTos>>
    {
        public Guid Id { get; set; }
        
        public int PageNumber { get; set;}
        
        public int PageSize { get; set; }
    }

