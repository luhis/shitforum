using Microsoft.AspNetCore.Mvc;
using System;

namespace ShitForum.ViewComponents
{

    public class FriendlyGuid : ViewComponent
    {
        public IViewComponentResult Invoke(Guid g) => View(g);
    }
}
