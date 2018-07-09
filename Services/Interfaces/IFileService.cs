using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Optional;

namespace Services.Interfaces
{
    public interface IFileService
    {
        Task<Option<File>> GetPostFile(Guid postId, CancellationToken cancellationToken);

        Task BanImage(ImageHash imageHash, string reason, CancellationToken cancellationToken);

        Task<IReadOnlyList<BannedImage>> GetAllBannedImages(CancellationToken cancellationToken);
    }
}
