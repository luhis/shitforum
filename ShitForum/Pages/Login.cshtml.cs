using System;
using System.Threading;
using Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShitForum.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ICookieStorage cookieStorage;
        private readonly AdminSettings adminSettings;

        public LoginModel(ICookieStorage cookieStorage, AdminSettings adminSettings)
        {
            this.cookieStorage = cookieStorage;
            this.adminSettings = adminSettings;
        }

        public IActionResult OnGet()
        {
            IActionResult Func() => Page().ToIAR();

            var authCookie = cookieStorage.ReadAdmin(this.HttpContext.Request);
            return authCookie.Match(some =>
            {
                var res = adminSettings.IsValid(some);
                return res.Match(_ => this.RedirectToPage<AdminModel>().ToIAR(), Func);
            }, Func);
        }

        [ValidateAntiForgeryToken]
        public IActionResult OnPost(Guid key, CancellationToken cancellationToken)
        {
            var res = adminSettings.IsValid(key);
            return res.Match(success =>
            {
                var (_, k) = success;
                cookieStorage.SetAdminCookie(this.HttpContext.Response, k);
                return this.RedirectToPage<AdminModel>().ToIAR();
            }, () =>
            {
                this.ModelState.AddModelError(string.Empty, "Incorrect key");
                return Page().ToIAR();
            });
        }
    }
}
