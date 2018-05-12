using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Optional;
using Services;
using ShitForum.Cookies;
using ShitForum.Models;

namespace ShitForum.Pages
{
    public class AdminModel : PageModel
    {
        private readonly ICookieStorage cookieStorage;
        private readonly AdminSettings adminSettings;
        private readonly IUserService userService;
        private readonly IFileService fileService;

        public AdminModel(ICookieStorage cookieStorage, AdminSettings adminSettings, IUserService userService, IFileService fileService)
        {
            this.cookieStorage = cookieStorage;
            this.adminSettings = adminSettings;
            this.userService = userService;
            this.fileService = fileService;
        }

        public AdminViewModel Model { get; private set; }

        public Task<IActionResult> OnGet()
        {
            Task<IActionResult> Func()
            {
                this.Model = new AdminViewModel("", Option.None<IReadOnlyList<BannedImage>>(), Option.None<IReadOnlyList<BannedIp>>());
                return Page().ToIART();
            }

            var authCookie = cookieStorage.ReadAdmin(this.HttpContext.Request);
            return authCookie.Match(some =>
            {
                var res = adminSettings.IsValid(some);
                return res.Match(async _ =>
                {
                    this.Model = new AdminViewModel("You are already logged in", Option.Some(await fileService.GetAllBannedImages()), Option.Some(await this.userService.GetAllBans()));
                    return Page().ToIAR();
                }, Func);
            }, Func);
        }


        [ValidateAntiForgeryToken]
        public Task<IActionResult> OnPost(Guid k)
        {
            var res = adminSettings.IsValid(k);
            return res.Match(async success =>
            {
                var (_, key) = success;
                this.Model = new AdminViewModel("You are now logged in", Option.Some(await fileService.GetAllBannedImages()), Option.Some(await this.userService.GetAllBans()));
                cookieStorage.SetAdminCookie(this.HttpContext.Response, key);
                return Page().ToIAR();
            }, () =>
            {
                this.ModelState.AddModelError(string.Empty, "Incorrect key");
                return Page().ToIART();
            });
        }
    }
}
