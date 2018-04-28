using System;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Optional;

namespace Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository fileRepository;
        private readonly IBannedImageRepository bannedImageRepository;

        public FileService(IFileRepository fileRepository, IBannedImageRepository bannedImageRepository)
        {
            this.fileRepository = fileRepository;
            this.bannedImageRepository = bannedImageRepository;
        }

        Task<Option<File>> IFileService.GetPostFile(Guid postId)
        {
            return fileRepository.GetPostFile(postId);
        }

        Task IFileService.BanImage(ImageHash imageHash, string reason)
        {
            return this.bannedImageRepository.Ban(imageHash, reason);
        }
    }
}
