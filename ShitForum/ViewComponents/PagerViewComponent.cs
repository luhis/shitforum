using Microsoft.AspNetCore.Mvc;
using Services.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShitForum.ViewComponents
{
    public class PagerViewComponent : ViewComponent
    {
        public PageData PageData { get; private set; }
        public List<int> PagesToShow { get; private set; }

        public Task<IViewComponentResult> InvokeAsync(int pageNumber)
        {
            ////this.PageData = pageData;
            ////var ps = new List<int>() { pageData.PageNumber - 1, pageData.PageNumber, pageData.PageNumber + 1 }.Where(a => a >= 1 && a <= pageData.NumberOfPages);
            ////this.PagesToShow = ps.ToList();
            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}
