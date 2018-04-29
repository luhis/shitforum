using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ShitForum.ViewComponents
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
