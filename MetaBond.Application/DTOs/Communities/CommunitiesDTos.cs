using MetaBond.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.DTOs.Communties
{
    public sealed record CommunitiesDTos
    (
        Guid CommunitieId,
        string? Name,
        string? Category,
        DateTime CreatedAt
    );
}
