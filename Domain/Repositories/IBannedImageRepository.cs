using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IBannedImageRepository
    {
        [Pure]
        Task<bool> IsBanned(ImageHash hash, CancellationToken cancellationToken);

        [Pure]
        Task Ban(ImageHash hash, string reason, CancellationToken cancellationToken);

        [Pure]
        Task<IReadOnlyList<BannedImage>> GetAll(CancellationToken cancellationToken);
    }
}
