using System;
using System.Threading;
using System.Threading.Tasks;
using Optional;
using Services.Dtos;

namespace Services.Interfaces
{
    public interface IThreadService
    {
        Task<Option<ThreadOverViewSet>> GetOrderedThreads(string boardKey, Option<string> filter, int pageSize, int pageNumber, CancellationToken cancellationToken);

        Task<Option<CatalogThreadOverViewSet>> GetOrderedCatalogThreads(string boardKey, CancellationToken cancellationToken);

        Task<Option<ThreadDetailView>> GetThread(Guid threadId, int pageSize, CancellationToken cancellationToken);
    }
}
