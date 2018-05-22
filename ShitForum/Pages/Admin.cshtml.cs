using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using ShitForum.Attributes;
using ShitForum.Cookies;
using ShitForum.Models;

namespace ShitForum.Pages
{
    [CookieAuth]
    public class AdminModel : PageModel
    {
        private readonly ICookieStorage cookieStorage;
        private readonly AdminSettings adminSettings;
        private readonly IUserService userService;
        private readonly IFileService fileService;
        private readonly IBoardService boardService;

        public AdminModel(ICookieStorage cookieStorage, AdminSettings adminSettings, IUserService userService,
            IFileService fileService, IBoardService boardService)
        {
            this.cookieStorage = cookieStorage;
            this.adminSettings = adminSettings;
            this.userService = userService;
            this.fileService = fileService;
            this.boardService = boardService;
        }

        public AdminViewModel Model { get; private set; }

        [BindProperty] public AddBoard Board { get; set; }

        public Task<IActionResult> OnGet(CancellationToken cancellationToken)
        {
            Task<IActionResult> Func()
            {
                return RedirectToPage("Login").ToIART();
            }

            var authCookie = cookieStorage.ReadAdmin(this.HttpContext.Request);
            return authCookie.Match(some =>
            {
                var res = adminSettings.IsValid(some);
                return res.Match(async _ =>
                {
                    var boards = await this.boardService.GetAll(cancellationToken);
                    var bannedImages = await fileService.GetAllBannedImages(cancellationToken);
                    var banns = await this.userService.GetAllBans(cancellationToken);

                    this.Model = new AdminViewModel(bannedImages, banns, boards);
                    return Page().ToIAR();
                }, Func);
            }, Func);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDelete(Guid boardId, CancellationToken cancellationToken)
        {
            await this.boardService.Delete(boardId, cancellationToken);
            return RedirectToPage();
        }
        
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAddBoard(CancellationToken cancellationToken)
        {
            if (this.ModelState.IsValid)
            {
                await this.boardService.Add(Guid.NewGuid(), this.Board.BoardName, this.Board.BoardKey);
                return RedirectToPage();
            }
            else
            {
                var boards = await this.boardService.GetAll(cancellationToken);
                var bannedImages = await fileService.GetAllBannedImages(cancellationToken);
                var banns = await this.userService.GetAllBans(cancellationToken);

                this.Model = new AdminViewModel(bannedImages, banns, boards);
                return Page();
            }
        }
    }
}
