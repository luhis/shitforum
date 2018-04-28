using System;
using EnsureThat;

namespace Services.Dtos
{
    public sealed class PostDetailView
    {
        public PostDetailView( Guid id,Guid threadId, string threadSubject, BoardOverView board, string comment)
        {
            this.Id = EnsureArg.IsNotEmpty(id, nameof(id));
            ThreadId = EnsureArg.IsNotEmpty(threadId, nameof(threadId));
            ThreadSubject = EnsureArg.IsNotNull(threadSubject, nameof(threadSubject));
            Board = EnsureArg.IsNotNull(board, nameof(board));
            Comment = EnsureArg.IsNotNull(comment, nameof(comment));
        }

        public Guid Id { get; }
        public Guid ThreadId { get; }
        public string ThreadSubject { get; }
        public BoardOverView Board { get; }
        public string Comment { get; }
    }
}
