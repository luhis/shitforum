using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Dtos;
using Services.Interfaces;
using ShitForum.Attributes;

namespace ShitForum.Pages
{
    [ServiceFilter(typeof(CookieAuthAttribute))]
    public class DeletePostModel : PageModel
    {
        private readonly IPostService postService;
        public PostContextView Post { get; private set; }

        public DeletePostModel(IPostService postService)
        {
            this.postService = postService;
        }

        public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotEmpty(id, nameof(id));
            var p = await postService.GetById(id, cancellationToken);
            return p.Match(post =>
            {
                this.Post = post;
                return Page().ToIAR();
            }, () => this.NotFound().ToIAR());
        }
        
        public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotEmpty(id, nameof(id));
            var p = await postService.GetById(id, cancellationToken);
            return await p.Match(async post =>
            {
                await postService.DeletePost(id, cancellationToken);
                return new RedirectToPageResult("Thread", new { boardKey = post.Board.BoardKey, threadId = post.ThreadId }).ToIAR();
            }, () => new StatusCodeResult(StatusCodes.Status500InternalServerError).ToIART());
        }
    }
}
