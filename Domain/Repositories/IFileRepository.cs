using System;
using System.Threading.Tasks;
using Optional;

namespace Domain.Repositories
{
    public interface IFileRepository
    {
        Task<Option<File>> GetPostFile(Guid postId);

        Task<int> GetImageCount(Guid threadId);

        Task Add(File file);
    }
}