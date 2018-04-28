using System;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Services.Dtos;
using ShitForum.Attributes;
using ShitForum.Hasher;

namespace ShitForum.Pages
{
    [CookieAuth]
    public class BanImageModel : PageModel
    {
        private readonly IPostService postService;
        private readonly IFileService fileService;

        public BanImageModel(IFileService fileService, IPostService postService)
        {
            this.fileService = fileService;
            this.postService = postService;
        }

        [BindProperty] public string Reason { get; set; }
        public PostDetailView Post { get; private set; }

        public async Task<IActionResult> OnGet(Guid id)
        {
            EnsureArg.IsNotEmpty(id, nameof(id));
            var f = await this.fileService.GetPostFile(id);
            return await f.Match(async some =>
            {
                var p = await this.postService.GetById(some.PostId);
                return p.Match(post =>
                {
                    this.Post = post;
                    return Page();
                }, new NotFoundResult().ToIAR);
            }, () => new NotFoundResult().ToIART());
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            EnsureArg.IsNotEmpty(id, nameof(id));
            var f = await this.fileService.GetPostFile(id);

            return await f.Match(async some =>
            {
                var hash = ImageHasher.Hash(some.Data);
                await fileService.BanImage(hash, Reason);
                return base.RedirectToPage("Index").ToIAR();
            }, () => new NotFoundResult().ToIART());
        }
    }
}
