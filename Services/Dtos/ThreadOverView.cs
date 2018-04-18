using System;
using System.Collections.Generic;
using EnsureThat;

namespace Services.Dtos
{
    public sealed class ThreadOverView 
    {
        public ThreadOverView(Guid threadId, string subject, PostOverView firstPost, IReadOnlyList<PostOverView> finalPosts, int postCount, int imageCount)
        {
            this.ThreadId = EnsureArg.IsNotEmpty(threadId, nameof(threadId));
            Subject = EnsureArg.IsNotNull(subject, nameof(subject));
            FirstPost = EnsureArg.IsNotNull(firstPost, nameof(firstPost));
            FinalPosts = EnsureArg.IsNotNull(finalPosts, nameof(finalPosts));
            PostCount = EnsureArg.IsGte(postCount, 1, nameof(PostCount));
            ImageCount = EnsureArg.IsGte(imageCount, 1, nameof(ImageCount));
        }

        public Guid ThreadId { get; }

        public string Subject { get; }

        public PostOverView FirstPost { get; }

        public IReadOnlyList<PostOverView> FinalPosts { get; }

        public int PostCount { get; }

        public int ImageCount { get; }
    }
}