using Microsoft.AspNetCore.Http;

namespace ShitForum
{
    public interface ICookieStorage
    {
        void SetNameCookie(HttpResponse r, string name);
        string ReadName(HttpRequest r);
    }
}