using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ShitForum.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IBoardService boardService;

        public IEnumerable<Board> Boards { get; private set; }

        public IndexModel(IBoardService boardService)
        {
            this.boardService = boardService;
        }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            this.Boards = await this.boardService.GetAll(cancellationToken).ConfigureAwait(false);
        }
    }
}
