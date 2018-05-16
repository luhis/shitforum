using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IBannedImageRepository
    {
        Task<bool> IsBanned(ImageHash hash, CancellationToken cancellationToken);

        Task Ban(ImageHash hash, string reason, CancellationToken cancellationToken);

        Task<IReadOnlyList<BannedImage>> GetAll(CancellationToken cancellationToken);
    }
}
