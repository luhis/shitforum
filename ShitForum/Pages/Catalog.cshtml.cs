using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Services.Dtos;

namespace ShitForum.Pages
{
    public class CatalogModel : PageModel
    {
        private readonly IThreadService threadService;

        public CatalogModel(IThreadService threadService)
        {
            this.threadService = threadService;
        }

        public async Task<IActionResult> OnGet(string boardKey)
        {
            EnsureArg.IsNotEmpty(boardKey, nameof(boardKey));
            var t = await this.threadService.GetOrderedCatalogThreads(boardKey);
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
