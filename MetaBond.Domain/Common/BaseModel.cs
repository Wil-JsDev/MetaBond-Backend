using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Domain.Common;

    public class BaseModel
    {
       public Guid Id { get; set; }

       public string? Name { get; set; }
    }

