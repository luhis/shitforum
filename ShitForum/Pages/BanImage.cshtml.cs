using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using ShitForum.Attributes;
using ShitForum.Hasher;

namespace ShitForum.Pages
{
    [CookieAuth]
    public class BanImageModel : PageModel
    {
        private readonly IFileService fileService;

        public BanImageModel(IFileService fileService)
        {
            this.fileService = fileService;
        }

        [BindProperty] public string Reason { get; set; }

        public async Task<IActionResult> OnGet(Guid id)
        {
            var f = await this.fileService.GetPostFile(id);
            return f.Match(some => Page(), () => new NotFoundResult().ToIAR());
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
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
