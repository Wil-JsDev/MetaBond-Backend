﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace MetaBond.Domain.Models;

    public sealed class Friendship
    {
        public Guid Id { get; set; }
           
        public Status Status { get; set; }    

        public DateTime? CreateAt { get; set; }
    }
