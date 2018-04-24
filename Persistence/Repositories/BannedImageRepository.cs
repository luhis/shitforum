using System;
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

        Task IBannedImageRepository.Ban(ImageHash hash, string reason)
        {
            this.client.BannedImages.Add(new BannedImage(Guid.NewGuid(), hash.Val, reason));
            return this.client.SaveChangesAsync();
        }

        Task<bool> IBannedImageRepository.IsBanned(ImageHash hash)
        {
            return this.client.BannedImages.AnyAsync(a => a.Hash == hash.Val);
        }
    }
}
