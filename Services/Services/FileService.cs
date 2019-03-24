using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Optional;
using Services.Interfaces;

namespace Services.Services
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

        Task<Option<File>> IFileService.GetPostFile(Guid postId, CancellationToken cancellationToken)
        {
            return this.fileRepository.GetPostFile(postId, cancellationToken);
        }

        Task IFileService.BanImage(ImageHash imageHash, string reason, CancellationToken cancellationToken)
        {
            return this.bannedImageRepository.Ban(imageHash, reason, cancellationToken);
        }

        Task<IReadOnlyList<BannedImage>> IFileService.GetAllBannedImages(CancellationToken cancellationToken)
        {
            return this.bannedImageRepository.GetAll(cancellationToken);
        }
    }
}
