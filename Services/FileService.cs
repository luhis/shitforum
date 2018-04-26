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

        public FileService(IFileRepository fileRepository)
        {
            this.fileRepository = fileRepository;
        }

        Task<Option<File>> IFileService.GetPostFile(Guid postId)
        {
            return fileRepository.GetPostFile(postId);
        }
    }
}