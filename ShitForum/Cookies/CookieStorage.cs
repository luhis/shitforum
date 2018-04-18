using Microsoft.AspNetCore.Http;

namespace ShitForum
{
    public class CookieStorage : ICookieStorage
    {
        private const string CookieName = "name";

        void ICookieStorage.SetNameCookie(HttpResponse r,string name)
        {
            r.Cookies.Append(CookieName, name);
        }

        string ICookieStorage.ReadName(HttpRequest r) => r.Cookies[CookieName];
    }
}