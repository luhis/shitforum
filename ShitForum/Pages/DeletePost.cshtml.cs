using System;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Services.Dtos;
using ShitForum.Attributes;

namespace ShitForum.Pages
{
    [CookieAuthAttribute]
    public class DeletePostModel : PageModel
    {
        private readonly IPostService postService;
        public PostContextView Post { get; private set; }

        public DeletePostModel(IPostService postService)
        {
            this.postService = postService;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            EnsureArg.IsNotEmpty(id, nameof(id));
            var p = await postService.GetById(id);
            return p.Match(post =>
            {
                this.Post = post;
                return Page().ToIAR();
            }, () => new NotFoundResult().ToIAR());
        }
        
        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            EnsureArg.IsNotEmpty(id, nameof(id));
            var p = await postService.GetById(id);
            return await p.Match(async post =>
            {
                await postService.DeletePost(id);
                return new RedirectToPageResult("Thread", new { boardKey = post.Board.BoardKey, threadId = post.ThreadId }).ToIAR();
            }, () => new StatusCodeResult(StatusCodes.Status500InternalServerError).ToIART());
        }
    }
}
