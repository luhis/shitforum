using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShitForum
{
    public static class PageModelExtensions
    {
        public static RedirectToPageResult RedirectToPage<T>(this PageModel m, object routeValues = null) where T : PageModel
        {
            var pageName = RemoveFromEnd(typeof(T).Name, "Model");
            return m.RedirectToPage(pageName, routeValues);
        }

        private static string RemoveFromEnd(this string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }
            else
            {
                return s;
            }
        }
    }
}
