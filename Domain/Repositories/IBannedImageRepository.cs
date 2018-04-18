
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IBannedImageRepository
    {
        Task<bool> IsBanned(ImageHash hash);

        Task Ban(ImageHash hash, string reason);
    }
}