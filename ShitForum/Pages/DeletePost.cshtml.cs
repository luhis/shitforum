using System;
using System.Threading.Tasks;
using Domain;
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
        private readonly IThreadService threadService;
        public Post Post { get; private set; }
        public ThreadDetailView Thread { get; private set; }

        public DeletePostModel(IPostService postService, IThreadService threadService)
        {
            this.postService = postService;
            this.threadService = threadService;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var p = await postService.GetById(id);
            return await p.Match(async post =>
            {
                var t = await threadService.GetThread(post.ThreadId);
                return t.Match(thread =>
                {
                    this.Thread = thread;
                    this.Post = post;
                    return Page().ToIAR();
                }, () => new NotFoundResult().ToIAR());
            }, () => new NotFoundResult().ToIART());
        }
        
        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            var p = await postService.GetById(id);
            return await p.Match(async post =>
            {
                await postService.DeletePost(post.Id);
                return new RedirectToPageResult("Thread", new { id = post.ThreadId }).ToIAR();
            }, () => new StatusCodeResult(StatusCodes.Status500InternalServerError).ToIART());
        }
    }
}