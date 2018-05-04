using Microsoft.AspNetCore.Mvc;
using Services.Dtos;

namespace ShitForum.ViewComponents
{
    public class DateTimeView : ViewComponent
    {
        public IViewComponentResult Invoke(PostOverView p) => View(p.Created);
    }
}
