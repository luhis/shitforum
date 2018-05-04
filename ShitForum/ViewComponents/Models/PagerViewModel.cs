using System.Collections.Generic;

namespace ShitForum.ViewComponents.Models
{
    public class PagerViewModel
    {
        public PagerViewModel(int pageNumber, IReadOnlyList<int> pagesToShow, int numberOfPages)
        {
            PageNumber = pageNumber;
            PagesToShow = pagesToShow;
            NumberOfPages = numberOfPages;
        }

        public int PageNumber { get; }
        public IReadOnlyList<int> PagesToShow { get; }
        public int NumberOfPages { get; }
    }
}
