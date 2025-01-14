using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace MetaBond.Domain.Models;

    public sealed class Friendship
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }
           
        public enum Status { Pendiete, Aceptada, Bloqueada }    

        public DateTime? CreateAt { get; set; }
    }
