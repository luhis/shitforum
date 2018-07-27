using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("[action]/{postId}")]
        public async Task<IActionResult> GetPostThumbnail(Guid postId, CancellationToken cancellationToken)
        {
            var post = await this.fileRepository.GetPostFile(postId, cancellationToken).ConfigureAwait(false);
            return post.Match(some => File(some.ThumbNailJpeg, "image/jpeg").ToIAR(), () => new NotFoundResult());
        }
        
        [HttpGet("[action]/{postId}")]
        public async Task<IActionResult> GetPostImage(Guid postId, CancellationToken cancellationToken)
        {
            var post = await this.fileRepository.GetPostFile(postId, cancellationToken).ConfigureAwait(false);
            return post.Match(some => File(some.Data, some.MimeType).ToIAR(), () => new NotFoundResult());
        }
    }
}
