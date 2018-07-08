using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Optional;

namespace Cookies
{
    public class CookieStorage : ICookieStorage
    {
        public CookieStorage(IHostingEnvironment env)
        {
            this.options = new CookieOptions() {HttpOnly = true, Secure = !env.IsDevelopment()};
        }

        private const string CookieName = "name";
        private const string CookieAdmin = "adminKey";
        private const string ThumbPrint = "thumbPrint";
        private readonly CookieOptions options;

        void ICookieStorage.SetNameCookie(HttpResponse r,string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                r.Cookies.Delete(CookieName);
            }
            else
            {
                r.Cookies.Append(CookieName, name, options);
            }
        }

        string ICookieStorage.ReadName(HttpRequest r) => r.Cookies[CookieName];

        void ICookieStorage.SetAdminCookie(HttpResponse r, Guid key)
        {
            r.Cookies.Append(CookieAdmin, key.ToString(), options);
        }

        Option<Guid> ICookieStorage.ReadThumbPrint(HttpRequest r)
        {
            var c = r.Cookies[ThumbPrint];
            if (!string.IsNullOrWhiteSpace(c))
            {
                return Option.Some(new Guid(c));
            }
            else
            {
                return Option.None<Guid>();
            }
        }

        void ICookieStorage.SetThumbPrint(HttpResponse r, Guid thumbprint)
        {
            r.Cookies.Append(ThumbPrint, thumbprint.ToString(), options);
        }

        Option<Guid> ICookieStorage.ReadAdmin(HttpRequest r)
        {
            var c = r.Cookies[CookieAdmin];
            if (!string.IsNullOrWhiteSpace(c))
            {
                return  Option.Some(new Guid(c));
            }
            else
            {
                return Option.None<Guid>();
            }
        }
    }
}
