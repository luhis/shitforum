using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShitForum.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IBoardRepository boardRepository;
        public IEnumerable<Board> Boards { get; private set; }

        public IndexModel(IBoardRepository boardRepository)
        {
            this.boardRepository = boardRepository;
        }

        public async Task OnGetAsync()
        {
            this.Boards = await this.boardRepository.GetAll().ConfigureAwait(false);
        }
    }
}
