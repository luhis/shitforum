using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ShitForum.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IBoardService boardRepository;

        public IEnumerable<Board> Boards { get; private set; }

        public IndexModel(IBoardService boardRepository)
        {
            this.boardRepository = boardRepository;
        }

        public async Task OnGetAsync()
        {
            this.Boards = await this.boardRepository.GetAll().ConfigureAwait(false);
        }
    }
}
