using System;
using System.Threading.Tasks;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ShitForum.ApiControllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        private readonly IFileRepository postRepository;

        public ImagesController(IFileRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetPostThumbnail(Guid id)
        {
            var post = await this.postRepository.GetPostFile(id).ConfigureAwait(false);
            return post.Match(some => File(some.ThumbNailJpeg, "image/jpeg").ToIAR(), () => new NotFoundResult());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetPostImage(Guid id)
        {
            var post = await this.postRepository.GetPostFile(id).ConfigureAwait(false);
            return post.Match(some => File(some.Data, some.MimeType).ToIAR(), () => new NotFoundResult());
        }
    }
}
