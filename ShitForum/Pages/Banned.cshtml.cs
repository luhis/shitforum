using System;
using System.Threading;
using System.Threading.Tasks;
using Hashers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using ShitForum.GetIp;

namespace ShitForum.Pages
{
    public class BannedModel : PageModel
    {
        private readonly IUserService userService;
        private readonly IIpHasher hasher;
        private readonly IGetIp getIp;

        public BannedModel(IUserService userService, IIpHasher hasher, IGetIp getIp)
        {
            this.userService = userService;
            this.hasher = hasher;
            this.getIp = getIp;
        }

        public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
        {
            var ip = this.getIp.GetIp(this.Request);
            var ipHash = hasher.Hash(ip);
            var expiry = await this.userService.GetExpiry(ipHash, cancellationToken);
            return expiry.Match(some =>
            {
                this.Expiry = some;
                return this.Page().ToIAR();
            }, this.NotFound);
        }

        public DateTime Expiry { get; private set; }
    }
}
