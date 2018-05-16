using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class BannedImageRepository : IBannedImageRepository
    {
        private readonly ForumContext client;

        public BannedImageRepository(ForumContext client)
        {
            this.client = client;
        }

        Task IBannedImageRepository.Ban(ImageHash hash, string reason, CancellationToken cancellationToken)
        {
            this.client.BannedImages.Add(new BannedImage(Guid.NewGuid(), hash.Val, reason));
            return this.client.SaveChangesAsync(cancellationToken);
        }

        Task<IReadOnlyList<BannedImage>> IBannedImageRepository.GetAll(CancellationToken cancellationToken)
        {
            return this.client.BannedImages.ToReadOnlyAsync(cancellationToken);
        }

        Task<bool> IBannedImageRepository.IsBanned(ImageHash hash, CancellationToken cancellationToken)
        {
            return this.client.BannedImages.AnyAsync(a => a.Hash == hash.Val, cancellationToken);
        }
    }
}
