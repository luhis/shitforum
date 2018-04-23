using System;
using Microsoft.AspNetCore.Http;
using Optional;

namespace ShitForum.Cookies
{
    public interface ICookieStorage
    {
        void SetNameCookie(HttpResponse r, string name);
        string ReadName(HttpRequest r);
        Option<Guid> ReadAdmin(HttpRequest r);
        void SetAdminCookie(HttpResponse r, Guid key);
    }
}