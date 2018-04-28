using System;
using System.Threading.Tasks;
using Domain;
using Optional;

namespace Services
{
    public interface IFileService
    {
        Task<Option<File>> GetPostFile(Guid postId);
        Task BanImage(ImageHash imageHash, string reason);
    }
}
