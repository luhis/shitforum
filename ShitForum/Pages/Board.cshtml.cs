using System;
using System.Threading;
using System.Threading.Tasks;
using Cookies;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Dtos;
using ShitForum.Attributes;
using ShitForum.BannedImageLogger;
using ShitForum.GetIp;
using Hashers;
using Services.Interfaces;
using ShitForum.Mappers;
using ShitForum.Models;
using static ShitForum.PageModelExtensions;

namespace ShitForum.Pages
{
    [Recaptcha]
    public class BoardModel : PageModel
    {
        private readonly IIpHasher ipHasher;
        private readonly TripCodeHasher tripCodeHasher;
        private readonly ICookieStorage cookieStorage;
        private readonly IGetIp getIp;
        private readonly IThreadService threadService;
        private readonly IPostService postService;
        private readonly IBannedImageLogger bannedImageLogger;
        private readonly IUploadMapper uploadMapper;

        public BoardModel(
            IpHasherFactory ipHasherFactory,
            TripCodeHasher tripCodeHasher,
            ICookieStorage cookieStorage,
            IGetIp getIp,
            IThreadService threadService,
            IPostService postService,
            IBannedImageLogger bannedImageLogger,
            IUploadMapper uploadMapper)
        {
            this.ipHasher = ipHasherFactory.GetHasher();
            this.tripCodeHasher = tripCodeHasher;
            this.cookieStorage = cookieStorage;
            this.getIp = getIp;
            this.threadService = threadService;
            this.postService = postService;
            this.bannedImageLogger = bannedImageLogger;
            this.uploadMapper = uploadMapper;
        }

        public async Task<IActionResult> OnGet(string boardKey, string filter, CancellationToken cancellationToken, int pageNumber = 1)
        {
            EnsureArg.IsNotNull(boardKey, nameof(boardKey));
            this.Filter = filter;
            var filterOption = NullableMapper.ToOption(filter);
            var t = await this.threadService.GetOrderedThreads(boardKey, filterOption, 100, pageNumber, cancellationToken);
            return t.Match(ts =>
            {
                this.Threads = ts;
                this.Thread = new AddThread(ts.Board.Id, cookieStorage.ReadName(this.Request), string.Empty, string.Empty, string.Empty, null);
                return Page().ToIAR();
            },
            () => new NotFoundResult().ToIAR());
        }

        public ThreadOverViewSet Threads { get; private set; }

        [BindProperty]
        public AddThread Thread { get; set; }

        public string Filter { get; private set; }

        [Recaptcha]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync(string boardKey, string filter, CancellationToken cancellationToken, int pageNumber = 1)
        {
            var ip = this.getIp.GetIp(this.Request);
            var ipHash = this.ipHasher.Hash(ip);

            this.bannedImageLogger.Log(this.ModelState[nameof(this.Thread.File)], ip, ipHash);

            var filterOption = NullableMapper.ToOption(filter);

            var t = await this.threadService.GetOrderedThreads(boardKey, filterOption, Constants.PageSize, pageNumber, cancellationToken);
            return await t.Match(async threads =>
            {
                if (!ModelState.IsValid)
                {
                    this.Threads = threads;
                    return Page().ToIAR();
                }

                var threadId = Guid.NewGuid();
                var postId = Guid.NewGuid();
                var trip = tripCodeHasher.Hash(StringFuncs.MapString(this.Thread.Name, "anonymous"));
                var options = OptionsMapper.Map(this.Thread.Options);
                var f = uploadMapper.Map(this.Thread.File, postId);

                var result = await this.postService.AddThread(postId, threadId, this.Thread.BoardId, this.Thread.Subject ?? string.Empty, trip, this.Thread.Comment, options.Sage, ipHash, f, cancellationToken);
                
                return result.Match(
                    _ =>
                    {
                        this.cookieStorage.SetNameCookie(this.Response, this.Thread.Name);
                        if (options.NoNoko)
                        {
                            return this.RedirectToPage<BoardModel>(new { boardKey = boardKey }).ToIAR();
                        }
                        else
                        {
                            return this.RedirectToPage<ThreadModel>(new { threadId = threadId }).ToIAR();
                        }
                    },
                    _ => this.RedirectToPage<BannedModel>().ToIAR());
            }, () => this.NotFound().ToIART());
        }
    }
}
