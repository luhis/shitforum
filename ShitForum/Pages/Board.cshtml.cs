using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Optional;
using ReCaptchaCore;
using Services;
using Services.Dtos;
using ShitForum.ApiControllers;
using ShitForum.Hasher;
using ShitForum.ImageValidation;
using ShitForum.Mappers;
using ShitForum.Models;

namespace ShitForum.Pages
{
    public class BoardModel : PageModel
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

        public BoardModel(
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

        public async Task<IActionResult> OnGet(Guid id, string filter)
        {
            EnsureArg.IsNotEmpty(id, nameof(id));
            this.Filter = filter;
            var filterOption = NullableMapper.ToOption(filter);
            var t = await this.threadService.GetOrderedThreads(id, filterOption, 100, 0);
            return t.Match(ts =>
            {
                this.Threads = ts.Threads;
                this.Thread = new AddThread() { BoardId = id, Name = cookieStorage.ReadName(this.Request) };
                this.Board = ts.Board;
                return Page().ToIAR();
            },
            () => new NotFoundResult().ToIAR());
        }

        public IEnumerable<ThreadOverView> Threads { get; private set; }

        [BindProperty]
        public AddThread Thread { get; set; }

        public Board Board { get; private set; }

        public string Filter { get; private set; }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync(string filter)
        {
            var ip = this.getIp.GetIp(this.Request);
            var filterOption = NullableMapper.ToOption(filter);
            var ipHash = this.ipHasher.Hash(ip);
            await validateImage.ValidateAsync(
                UploadMapper.ExtractData(this.Thread.File),
                ip,
                ipHash,
                s => this.ModelState.AddModelError(nameof(this.Thread.File), s));

            var recaptcha = this.getCaptchaValue.Get(this.Request);
            if (!await this.recaptchaVerifier.IsValid(recaptcha, ip))
            {
                this.ModelState.AddModelError(string.Empty, "Recaptcha is invalid");
            }

            if (!ModelState.IsValid)
            {
                var t = await this.threadService.GetOrderedThreads(this.Thread.BoardId, filterOption, 100, 0);
                return t.Match(some =>
                {
                    this.Threads = some.Threads;
                    this.Board = some.Board;
                    return Page().ToIAR();
                }, () => new NotFoundResult());
            }

            var threadId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var trip = tripCodeHasher.Hash(StringFuncs.MapString(this.Thread.Name, "anonymous"));
            var options = OptionsMapper.Map(this.Thread.Options);
            var f = UploadMapper.Map(this.Thread.File, postId);

            var result = await this.postService.AddThread(postId, threadId, this.Thread.BoardId, this.Thread.Subject ?? string.Empty, trip, this.Thread.Comment, options.Sage, ipHash, f);

            return result.Match(
               _ =>
               {
                   this.cookieStorage.SetNameCookie(this.Response, this.Thread.Name);
                   if (options.NoNoko)
                   {
                       return RedirectToPage("Board", new { id = this.Thread.BoardId }).ToIAR();
                   }
                   else
                   {
                       return RedirectToPage("Thread", new { id = threadId }).ToIAR();
                   }
               },
               _ => RedirectToPage("Banned").ToIAR());
        }
    }
}