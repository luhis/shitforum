using System.Collections.Generic;
using Domain;
using EnsureThat;

namespace Services.Dtos
{
    public class CatalogThreadOverViewSet
    {
        public CatalogThreadOverViewSet(Board board, IReadOnlyList<CatalogThreadOverView> threads)
        {
            Board = EnsureArg.IsNotNull(board, nameof(board));
            Threads = EnsureArg.IsNotNull(threads, nameof(threads));
        }

        public Board Board { get; }
        public IReadOnlyList<CatalogThreadOverView> Threads { get; }
    }
}
