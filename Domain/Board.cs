using System;
using System.Collections.Generic;
using EnsureThat;

namespace Domain
{
    public class Board : DomainBase
    {
        public Board(Guid id, string boardName, string boardKey) : base(id)
        {
            BoardName = EnsureArg.IsNotEmpty(boardName, nameof(boardName));
            BoardKey = EnsureArg.IsNotEmpty(boardKey, nameof(boardKey));
        }

        public Board()
        {
        }

        public string BoardName { get; private set; }

        public string BoardKey { get; private set; }

        public ICollection<Thread> Threads { get; private set; }
    }
}