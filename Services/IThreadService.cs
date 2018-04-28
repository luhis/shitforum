using Optional;
using System;
using System.Threading.Tasks;
using Services.Dtos;

namespace Services
{
    public interface IThreadService
    {
        Task<Option<ThreadOverViewSet>> GetOrderedThreads(string boardKey, Option<string> filter, int pageSize, int pageNumber);

        Task<Option<CatalogThreadOverViewSet>> GetOrderedCatalogThreads(string boardKey, int pageSize, int pageNumber);

        Task<Option<ThreadDetailView>> GetThread(Guid threadId);
    }
}
