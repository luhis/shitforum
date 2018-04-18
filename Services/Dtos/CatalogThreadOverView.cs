using System;
using Domain;
using EnsureThat;

namespace Services.Dtos
{
    public sealed class CatalogThreadOverView 
    {
        public CatalogThreadOverView(Guid threadId, string subject, Board board, PostOverView firstPost)
        {
            this.ThreadId = EnsureArg.IsNotEmpty(threadId, nameof(threadId));
            Board = EnsureArg.IsNotNull(board, nameof(board));
            Subject = EnsureArg.IsNotNull(subject, nameof(subject));
            FirstPost = EnsureArg.IsNotNull(firstPost, nameof(firstPost));
        }

        public Guid ThreadId { get; }

        public string Subject { get; }

        public Board Board { get; }

        public PostOverView FirstPost { get; }
    }
}