using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Dtos;
using Services.Interfaces;
using ShitForum.Attributes;

namespace ShitForum.Pages
{
    [CookieAuth]
    public class BanUserModel : PageModel
    {
        private readonly IUserService userService;
        private readonly IPostService postService;

        public BanUserModel(IUserService userService, IPostService postService)
        {
            this.userService = userService;
            this.postService = postService;
        }

        [BindProperty]
        public string Reason { get; set; }

        [BindProperty]
        public DateTime Expiry { get; set; }

        public PostContextView Post { get; private set; }

        public async Task<IActionResult> OnGet(Guid id, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotEmpty(id, nameof(id));
            var hash = await this.userService.GetHashForPost(id, cancellationToken);
            return await hash.Match(async some =>
            {
                var p = await this.postService.GetById(id, cancellationToken);
                return p.Match(post =>
                {
                    this.Post = post;
                    this.Expiry = DateTime.UtcNow.AddDays(7);
                    return Page().ToIAR();
                }, this.NotFound().ToIAR);
            }, () => this.NotFound().ToIART());
        }

        public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotEmpty(id, nameof(id));
            var hash = await this.userService.GetHashForPost(id, cancellationToken);
            return await hash.Match(async some =>
            {
                await userService.BanUser(some, Reason, Expiry);
                return RedirectToPage("Index").ToIAR();
            }, () => this.NotFound().ToIART());
        }
    }
}
