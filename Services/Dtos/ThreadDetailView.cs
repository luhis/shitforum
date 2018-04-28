using System;
using System.Collections.Generic;
using Domain;
using EnsureThat;

namespace Services.Dtos
{
    public sealed class ThreadDetailView
    {
        public ThreadDetailView(Guid threadId, string subject, BoardOverView board, IReadOnlyList<PostOverView> posts)
        {
            ThreadId = EnsureArg.IsNotEmpty(threadId, nameof(threadId));
            Subject = EnsureArg.IsNotNull(subject, nameof(subject));
            this.Board = EnsureArg.IsNotNull(board, nameof(board));
            Posts = EnsureArg.IsNotNull(posts, nameof(posts));
        }

        public Guid ThreadId { get; }

        public string Subject { get; }

        public BoardOverView Board { get; }

        public IReadOnlyList<PostOverView> Posts { get; }
    }
}
