using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ShitForum.Cookies
{
    public class CookieStorage : ICookieStorage
    {
        private readonly IHostingEnvironment env;

        public CookieStorage(IHostingEnvironment env)
        {
            this.env = env;
        }

        private const string CookieName = "name";
        private const string CookieAdmin = "adminKey";

        void ICookieStorage.SetNameCookie(HttpResponse r,string name)
        {
            r.Cookies.Append(CookieName, name, new CookieOptions() {HttpOnly = true, Secure=(!env.IsDevelopment()) });
        }

        string ICookieStorage.ReadName(HttpRequest r) => r.Cookies[CookieName];

        void ICookieStorage.SetAdminCookie(HttpResponse r, Guid key)
        {
            r.Cookies.Append(CookieAdmin, key.ToString(), new CookieOptions() { HttpOnly = true, Secure = (!env.IsDevelopment()) });
        }

        Guid ICookieStorage.ReadAdmin(HttpRequest r) => new Guid(r.Cookies[CookieAdmin]);

    }
}