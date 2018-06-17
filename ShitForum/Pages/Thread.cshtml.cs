using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Services.Dtos;
using ShitForum.Attributes;
using ShitForum.BannedImageLogger;
using ShitForum.Cookies;
using ShitForum.GetIp;
using Hashers;
using ShitForum.IsAdmin;
using ShitForum.Mappers;
using ShitForum.Models;

namespace ShitForum.Pages
{
    [Recaptcha]
    public class ThreadModel : PageModel
    {
        private readonly IIpHasher ipHasher;
        private readonly TripCodeHasher tripCodeHasher;
        private readonly ICookieStorage cookieStorage;
        private readonly IGetIp getIp;
        private readonly IThreadService threadService;
        private readonly IPostService postService;
        private readonly IBannedImageLogger bannedImageLogger;
        private readonly IIsAdmin isAdmin;
        private readonly IUploadMapper uploadMapper;

        public ThreadModel(
            IpHasherFactory ipHasherFactory,
            TripCodeHasher tripCodeHasher,
            ICookieStorage cookieStorage,
            IGetIp getIp,
            IThreadService threadService,
            IPostService postService,
            IBannedImageLogger bannedImageLogger,
            IIsAdmin isAdmin,
            IUploadMapper uploadMapper)
        {
            this.ipHasher = ipHasherFactory.GetHasher();
            this.tripCodeHasher = tripCodeHasher;
            this.cookieStorage = cookieStorage;
            this.getIp = getIp;
            this.threadService = threadService;
            this.postService = postService;
            this.bannedImageLogger = bannedImageLogger;
            this.isAdmin = isAdmin;
            this.uploadMapper = uploadMapper;
        }

        public ThreadDetailView Thread { get; private set; }
        public bool IsAdmin { get; private set; }

        [BindProperty] public AddPost Post { get; set; }

        public async Task<IActionResult> OnGet(string boardKey, Guid threadId, Guid replyTo, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotEmpty(threadId, nameof(threadId));
            var t = await this.threadService.GetThread(threadId, Constants.PageSize, cancellationToken).ConfigureAwait(false);
            return t.Match(thread =>
            {
                this.IsAdmin = this.isAdmin.IsAdmin(this.HttpContext);
                this.Thread = thread;
                var newComm = replyTo == Guid.Empty ? string.Empty : $">>{replyTo}\n";
                this.Post = new AddPost(threadId, this.cookieStorage.ReadName(this.Request), String.Empty, newComm, null);
                return Page().ToIAR();
            }, () => new NotFoundResult().ToIAR());
        }

        [Recaptcha]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            var ip = this.getIp.GetIp(this.Request);
            var ipHash = this.ipHasher.Hash(ip);

            var key = $"{nameof(this.Post)}.{nameof(this.Post.File)}";
            this.bannedImageLogger.Log(this.ModelState[key], ip, ipHash);

            var t = await this.threadService.GetThread(this.Post.ThreadId, Constants.PageSize, cancellationToken).ConfigureAwait(false);
            return await t.Match(async thread =>
            {
                if (!this.ModelState.IsValid)
                {
                    this.IsAdmin = this.isAdmin.IsAdmin(this.HttpContext);
                    this.Thread = thread;
                    return this.Page().ToIAR();
                }
                else
                {
                    var trip = tripCodeHasher.Hash(StringFuncs.MapString(this.Post.Name, "anonymous"));
                    var options = OptionsMapper.Map(this.Post.Options);
                    var postId = Guid.NewGuid();
                    var f = uploadMapper.Map(this.Post.File, postId);
                    var result = await this.postService.Add(postId, this.Post.ThreadId, trip, this.Post.Comment, options.Sage,
                        ipHash, f, cancellationToken);
                    return result.Match(
                        _ =>
                        {
                            this.cookieStorage.SetNameCookie(this.Response, this.Post.Name);
                            if (options.NoNoko)
                            {
                                return RedirectToPage("Board", new {boardKey = thread.Board.BoardKey});
                            }
                            else
                            {
                                return RedirectToPage("Thread", new { boardKey = thread.Board.BoardKey, threadId = this.Post.ThreadId }).ToIAR();
                            }
                        },
                        _ => RedirectToPage("Banned").ToIAR(),
                        _ =>
                        {
                            this.ModelState.AddModelError(string.Empty, "Image count exceeded");
                            return Page().ToIAR();
                        },
                        _ =>
                        {
                            this.ModelState.AddModelError(string.Empty, "Post count exceeded");
                            return Page().ToIAR();
                        });
                }
            }, () => new NotFoundResult().ToIART());
        }
    }
}
