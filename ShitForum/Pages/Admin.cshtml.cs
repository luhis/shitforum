using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShitForum.Cookies;

namespace ShitForum.Pages
{
    public class AdminModel : PageModel
    {
        private readonly ICookieStorage cookieStorage;
        private readonly AdminSettings adminSettings;

        public AdminModel(ICookieStorage cookieStorage, AdminSettings adminSettings)
        {
            this.cookieStorage = cookieStorage;
            this.adminSettings = adminSettings;
        }

        public IActionResult OnGet()
        {
            var authCookie = cookieStorage.ReadAdmin(this.HttpContext.Request);
            var res = adminSettings.IsValid(authCookie);
            return res.Match(_ =>
            {
                this.Message = "You are already logged in";
                return Page();
            }, Page);
        }

        public string Message { get; private set; }

        [ValidateAntiForgeryToken]
        public IActionResult OnPost(Guid key)
        {
            var res = adminSettings.IsValid(key);
            return res.Match(success =>
            {
                this.Message = "You are now logged in";
                cookieStorage.SetAdminCookie(this.HttpContext.Response, success.Item2);
                return Page();
            }, () =>
            {
                this.ModelState.AddModelError(string.Empty, "Incorrect key");
                return Page();
            });
        }
    }
}