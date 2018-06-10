using Microsoft.AspNetCore.Mvc;
using ShitForum.ViewComponents.Models;
using System.Linq;

namespace ShitForum.ViewComponents
{
    public class Pager : ViewComponent
    {
        public IViewComponentResult Invoke(int pageNumber, int numberOfPages)
        {
            var ps = Enumerable.Range(pageNumber - 2, 5).Where(a => a >= 1 && a <= numberOfPages).ToList();
            return View(new PagerViewModel(pageNumber, ps, numberOfPages));
        }
    }
}
