using System;
using System.Collections.Generic;
using EnsureThat;

namespace Domain
{
    public sealed class Thread : DomainBase
    {
        public Thread(Guid id, Guid boardId, string subject) : base(id)
        {
            BoardId = EnsureArg.IsNotEmpty(boardId, nameof(boardId));
            Subject = EnsureArg.IsNotNull(subject, nameof(subject));
        }

        public Thread()
        {
        }

        public string Subject { get; private set; }

        public Guid BoardId { get; private set; }

        public ICollection<Post> Posts { get; private set; }
    }
}
