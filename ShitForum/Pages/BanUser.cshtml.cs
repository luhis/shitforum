using System.Threading.Tasks;
using Domain.IpHash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using ShitForum.Attributes;

namespace ShitForum.Pages
{
    [CookieAuth]
    public class BanUserModel : PageModel
    {
        private readonly IUserService userService;

        public BanUserModel(IUserService userService)
        {
            this.userService = userService;
        }

        [BindProperty]
        public string Reason { get; set; }

        public void OnGet(string ipHash)
        {

        }

        public async Task<IActionResult> OnPostAsync(string ipHash)
        {
            await userService.BanUser(new IpHash(ipHash), Reason);
            return RedirectToPage("Index");
        }
    }
}