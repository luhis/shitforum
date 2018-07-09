using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interfaces;

namespace ShitForum.ApiControllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        private readonly IFileService fileRepository;

        public ImagesController(IFileService postRepository)
        {
            this.fileRepository = postRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetPostThumbnail(Guid id, CancellationToken cancellationToken)
        {
            var post = await this.fileRepository.GetPostFile(id, cancellationToken).ConfigureAwait(false);
            return post.Match(some => File(some.ThumbNailJpeg, "image/jpeg").ToIAR(), () => new NotFoundResult());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetPostImage(Guid id, CancellationToken cancellationToken)
        {
            var post = await this.fileRepository.GetPostFile(id, cancellationToken).ConfigureAwait(false);
            return post.Match(some => File(some.Data, some.MimeType).ToIAR(), () => new NotFoundResult());
        }
    }
}
