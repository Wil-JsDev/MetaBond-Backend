﻿using MetaBond.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MetaBond.Application.Feature.Friendship.Command.Delete
{
    public sealed class DeleteFriendshipCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
    }
}
