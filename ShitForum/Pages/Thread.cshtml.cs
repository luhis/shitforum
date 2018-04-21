using System;
using System.Threading.Tasks;
using Domain;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReCaptchaCore;
using Services;
using ShitForum.ApiControllers;
using ShitForum.Hasher;
using ShitForum.ImageValidation;
using ShitForum.Mappers;
using ShitForum.Models;

namespace ShitForum.Pages
{
    public class ThreadModel : PageModel
    {
        private readonly IIpHasher ipHasher;
        private readonly TripCodeHasher tripCodeHasher;
        private readonly ICookieStorage cookieStorage;
        private readonly IGetIp getIp;
        private readonly IThreadService threadService;
        private readonly IPostService postService;
        private readonly IValidateImage validateImage;
        private readonly IRecaptchaVerifier recaptchaVerifier;
        private readonly IGetCaptchaValue getCaptchaValue;

        public ThreadModel(
            IpHasherFactory ipHasherFactory, 
            TripCodeHasher tripCodeHasher, 
            ICookieStorage cookieStorage, 
            IGetIp getIp,
            IThreadService threadService,
            IPostService postService,
            IValidateImage validateImage,
            IRecaptchaVerifier recaptchaVerifier,
            IGetCaptchaValue getCaptchaValue)
        {
            this.ipHasher = ipHasherFactory.GetHasher();
            this.tripCodeHasher = tripCodeHasher;
            this.cookieStorage = cookieStorage;
            this.getIp = getIp;
            this.threadService = threadService;
            this.postService = postService;
            this.validateImage = validateImage;
            this.recaptchaVerifier = recaptchaVerifier;
            this.getCaptchaValue = getCaptchaValue;
        }

        public ViewThread Thread { get; private set; }
        public Board Board { get; private set; }

        [BindProperty]
        public AddPost Post { get; set; }

        public async Task<IActionResult> OnGet(Guid id, Guid replyTo)
        {
            EnsureArg.IsNotEmpty(id, nameof(id));
            var t = await this.threadService.GetThread(id).ConfigureAwait(false);
            return t.Match(thread =>
            {
                this.Thread = new ViewThread(thread.ThreadId, thread.Subject, thread.Posts);
                var newComm = replyTo == Guid.Empty ? string.Empty : $">>{replyTo}\n";
                this.Post = new AddPost() { ThreadId = id, Name = this.cookieStorage.ReadName(this.Request), Comment = newComm };
                this.Board = thread.Board;
                return Page().ToIAR();
            }, () => new NotFoundResult().ToIAR());
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            var ip = this.getIp.GetIp(this.Request);
            var ipHash = this.ipHasher.Hash(ip);
            await this.validateImage.ValidateAsync(
                UploadMapper.ExtractData(this.Post.File), 
                ip, 
                ipHash,
                s => this.ModelState.AddModelError(nameof(this.Post.File), s));

            var recaptcha = this.getCaptchaValue.Get(this.Request);
            if (!await this.recaptchaVerifier.IsValid(recaptcha, ip))
            {
                this.ModelState.AddModelError(string.Empty, "Recaptcha is invalid");
            }

            if (!this.ModelState.IsValid)
            {
                var t = await this.threadService.GetThread(this.Post.ThreadId).ConfigureAwait(false);
                return t.Match(thread =>
                {
                    this.Thread = new ViewThread(thread.ThreadId, thread.Subject, thread.Posts);
                    this.Board = thread.Board;
                    return this.Page().ToIAR();
                }, () => new NotFoundResult().ToIAR());
            }

            var trip = tripCodeHasher.Hash(StringFuncs.MapString(this.Post.Name, "anonymous"));
            var options = OptionsMapper.Map(this.Post.Options);
            var postId = Guid.NewGuid();
            var f = UploadMapper.Map(this.Post.File, postId);
            var result = await this.postService.Add(postId, this.Post.ThreadId, trip, this.Post.Comment, options.Sage, ipHash, f);
            return await result.Match(
                async _ =>
                {
                    this.cookieStorage.SetNameCookie(this.Response, this.Post.Name);
                    if (options.NoNoko)
                    {
                        var t = await this.threadService.GetThread(this.Post.ThreadId);
                        return t.Match(some => RedirectToPage("Board", new { id = some.Board.Id }), () => new NotFoundResult().ToIAR());
                    }
                    else
                    {
                        return RedirectToPage("Thread", new { id = this.Post.ThreadId }).ToIAR();
                    }
                }, 
                _ => Task.FromResult(RedirectToPage("Banned").ToIAR()), 
                _ => {
                    this.ModelState.AddModelError(string.Empty, "Image count exceeded");
                    return Task.FromResult(Page().ToIAR());
                },
                _ => {
                    this.ModelState.AddModelError(string.Empty, "Post count exceeded");
                    return Task.FromResult(Page().ToIAR());
                });
        }
    }
}