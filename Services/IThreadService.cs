using Optional;
using System;
using System.Threading.Tasks;
using Services.Dtos;

namespace Services
{
    public interface IThreadService
    {
        Task<Option<ThreadOverViewSet>> GetOrderedThreads(Guid boardId, Option<string> filter, int pageSize, int pageNumber);

        Task<Option<CatalogThreadOverViewSet>> GetOrderedCatalogThreads(Guid boardId, int pageSize, int pageNumber);

        Task<Option<ThreadDetailView>> GetThread(Guid threadId);
    }
}