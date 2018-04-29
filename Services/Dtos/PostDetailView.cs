using System;
using EnsureThat;

namespace Services.Dtos
{
    public sealed class PostContextView
    {
        public PostContextView(Guid threadId, string threadSubject, BoardOverView board, PostOverView post)
        {
            ThreadId = EnsureArg.IsNotEmpty(threadId, nameof(threadId));
            ThreadSubject = EnsureArg.IsNotNull(threadSubject, nameof(threadSubject));
            Board = EnsureArg.IsNotNull(board, nameof(board));
            Post = EnsureArg.IsNotNull(post, nameof(post));
        }
        
        public Guid ThreadId { get; }
        public string ThreadSubject { get; }
        public BoardOverView Board { get; }
        public PostOverView Post { get; }
    }
}
