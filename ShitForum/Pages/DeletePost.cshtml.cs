using System;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Services.Dtos;
using ShitForum.Cookies;

namespace ShitForum.Pages
{
    public class DeletePostModel : PageModel
    {
        private readonly ICookieStorage cookieStorage;
        private readonly AdminSettings adminSettings;
        private readonly IPostService postService;
        private readonly IThreadService threadService;
        public Post Post { get; private set; }
        public ThreadDetailView Thread { get; private set; }

        public DeletePostModel(ICookieStorage cookieStorage, AdminSettings adminSettings, IPostService postService, IThreadService threadService)
        {
            this.cookieStorage = cookieStorage;
            this.adminSettings = adminSettings;
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
            var cookie = cookieStorage.ReadAdmin(this.Request);
            if (adminSettings.IsValid(cookie).HasValue)
            {
                var p = await postService.GetById(id);
                return await p.Match(async post =>
                {
                    await postService.DeletePost(post.Id);
                    return new RedirectToPageResult("Thread", new { id = post.ThreadId }).ToIAR();
                }, () => new StatusCodeResult(StatusCodes.Status500InternalServerError).ToIART());
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }
    }
}