using System.Collections.Generic;
using Domain;
using EnsureThat;

namespace Services.Dtos
{
    public class ThreadOverViewSet
    {
        public ThreadOverViewSet(Board board, IReadOnlyList<ThreadOverView> threads)
        {
            Board = EnsureArg.IsNotNull(board, nameof(board));
            Threads = EnsureArg.IsNotNull(threads, nameof(threads));
        }

        public Board Board { get; }
        public IReadOnlyList<ThreadOverView> Threads { get; }
    }
}