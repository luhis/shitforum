using Microsoft.AspNetCore.Mvc;
using Services.Dtos;

namespace ShitForum.ViewComponents
{
    public class PostView : ViewComponent
    {
        public IViewComponentResult Invoke(PostOverView p) => View(p);
    }
}
