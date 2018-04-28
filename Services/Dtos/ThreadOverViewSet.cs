using System.Collections.Generic;
using Domain;
using EnsureThat;

namespace Services.Dtos
{
    public class ThreadOverViewSet
    {
        public ThreadOverViewSet(Board board, IReadOnlyList<ThreadOverView> threads, PageData pageData)
        {
            Board = EnsureArg.IsNotNull(board, nameof(board));
            Threads = EnsureArg.IsNotNull(threads, nameof(threads));
            PageData = EnsureArg.IsNotNull(pageData, nameof(pageData));
        }

        public Board Board { get; }
        public PageData PageData { get; }
        public IReadOnlyList<ThreadOverView> Threads { get; }
    }
}
