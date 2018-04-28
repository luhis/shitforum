using System;
using EnsureThat;

namespace Services.Dtos
{
    public sealed class BoardOverView
    {
        public BoardOverView(Guid boardId, string boardName, string boardKey)
        {
            BoardId = EnsureArg.IsNotEmpty(boardId, nameof(boardId));
            BoardName = EnsureArg.IsNotNull(boardName, nameof(boardName));
            this.BoardKey = EnsureArg.IsNotNull(boardKey, nameof(boardKey));
        }

        public Guid BoardId { get; }
        public string BoardName { get; }
        public string BoardKey { get; }
    }
}
