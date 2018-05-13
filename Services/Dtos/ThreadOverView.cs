using System;
using System.Collections.Generic;
using EnsureThat;

namespace Services.Dtos
{
    public sealed class ThreadOverView 
    {
        public ThreadOverView(Guid threadId, string subject, PostOverView firstPost, IReadOnlyList<PostOverView> finalPosts, ThreadOverViewStats stats)
        {
            this.ThreadId = EnsureArg.IsNotEmpty(threadId, nameof(threadId));
            Subject = EnsureArg.IsNotNull(subject, nameof(subject));
            OP = EnsureArg.IsNotNull(firstPost, nameof(firstPost));
            FinalPosts = EnsureArg.IsNotNull(finalPosts, nameof(finalPosts));
            Stats = EnsureArg.IsNotNull(stats, nameof(stats));
        }

        public Guid ThreadId { get; }

        public string Subject { get; }

        public PostOverView OP { get; }

        public IReadOnlyList<PostOverView> FinalPosts { get; }

        public ThreadOverViewStats Stats { get; }
    }
}
