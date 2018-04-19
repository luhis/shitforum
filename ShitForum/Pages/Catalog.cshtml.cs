using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Services.Dtos;
using ShitForum.ApiControllers;

namespace ShitForum.Pages
{
    public class CatalogModel : PageModel
    {
        private readonly IThreadService threadService;

        public CatalogModel(IThreadService threadService)
        {
            this.threadService = threadService;
        }

        public async Task<IActionResult> OnGet(Guid id)
        {
            EnsureArg.IsNotEmpty(id, nameof(id));
            var t = await this.threadService.GetOrderedCatalogThreads(id, 100, 0);
            return t.Match(threads =>
            {
                this.Threads = threads.Threads;
                this.Board = threads.Board;
                return Page().ToIAR();
            },
            () => new NotFoundResult().ToIAR());
        }

        public IEnumerable<CatalogThreadOverView> Threads { get; private set; }
        
        public Board Board { get; private set; }
    }
}