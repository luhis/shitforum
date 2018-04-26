using System;
using System.Threading.Tasks;
using Domain;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using ShitForum.Attributes;
using ShitForum.BannedImageLogger;
using ShitForum.Cookies;
using ShitForum.GetIp;
using ShitForum.Hasher;
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

        public ThreadModel(
            IpHasherFactory ipHasherFactory,
            TripCodeHasher tripCodeHasher,
            ICookieStorage cookieStorage,
            IGetIp getIp,
            IThreadService threadService,
            IPostService postService,
            IBannedImageLogger bannedImageLogger)
        {
            this.ipHasher = ipHasherFactory.GetHasher();
            this.tripCodeHasher = tripCodeHasher;
            this.cookieStorage = cookieStorage;
            this.getIp = getIp;
            this.threadService = threadService;
            this.postService = postService;
            this.bannedImageLogger = bannedImageLogger;
        }

        public ViewThread Thread { get; private set; }
        public Board Board { get; private set; }

        [BindProperty] public AddPost Post { get; set; }

        public async Task<IActionResult> OnGet(Guid id, Guid replyTo)
        {
            EnsureArg.IsNotEmpty(id, nameof(id));
            var t = await this.threadService.GetThread(id).ConfigureAwait(false);
            return t.Match(thread =>
            {
                this.Thread = new ViewThread(thread.ThreadId, thread.Subject, thread.Posts);
                var newComm = replyTo == Guid.Empty ? string.Empty : $">>{replyTo}\n";
                this.Post = new AddPost()
                {
                    ThreadId = id,
                    Name = this.cookieStorage.ReadName(this.Request),
                    Comment = newComm
                };
                this.Board = thread.Board;
                return Page().ToIAR();
            }, () => new NotFoundResult().ToIAR());
        }

        [Recaptcha]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            var ip = this.getIp.GetIp(this.Request);
            var ipHash = this.ipHasher.Hash(ip);

            var key = $"{nameof(this.Post)}.{nameof(this.Post.File)}";
            this.bannedImageLogger.Log(this.ModelState[key], ip, ipHash);

            var t = await this.threadService.GetThread(this.Post.ThreadId).ConfigureAwait(false);
            return await t.Match(async thread =>
            {
                if (!this.ModelState.IsValid)
                {
                    this.Thread = new ViewThread(thread.ThreadId, thread.Subject, thread.Posts);
                    this.Board = thread.Board;
                    return this.Page().ToIAR();
                }
                else
                {
                    var trip = tripCodeHasher.Hash(StringFuncs.MapString(this.Post.Name, "anonymous"));
                    var options = OptionsMapper.Map(this.Post.Options);
                    var postId = Guid.NewGuid();
                    var f = UploadMapper.Map(this.Post.File, postId);
                    var result = await this.postService.Add(postId, this.Post.ThreadId, trip, this.Post.Comment, options.Sage,
                        ipHash, f);
                    return result.Match(
                        _ =>
                        {
                            this.cookieStorage.SetNameCookie(this.Response, this.Post.Name);
                            if (options.NoNoko)
                            {
                                return RedirectToPage("Board", new {id = thread.Board.Id});
                            }
                            else
                            {
                                return RedirectToPage("Thread", new { id = this.Post.ThreadId }).ToIAR();
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