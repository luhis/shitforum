using System.Collections.Generic;
using Domain;
using EnsureThat;

namespace Services.Dtos
{

    public class PageData
    {
        public PageData(int pageNumber, int numberOfPages)
        {
            PageNumber = pageNumber;
            NumberOfPages = numberOfPages;
        }

        public int PageNumber { get; }
        public int NumberOfPages { get; }
    }
}
