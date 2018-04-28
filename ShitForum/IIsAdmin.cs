﻿using Microsoft.AspNetCore.Http;
using ShitForum.Cookies;

namespace ShitForum
{
    public interface IIsAdmin
    {
        bool IsAdmin(HttpContext httpContext);
    }

    public class IsAdmin : IIsAdmin
    {
        private readonly ICookieStorage cookieStorage;
        private readonly AdminSettings adminSettings;

        public IsAdmin(ICookieStorage cookieStorage, AdminSettings adminSettings)
        {
            this.cookieStorage = cookieStorage;
            this.adminSettings = adminSettings;
        }

        bool IIsAdmin.IsAdmin(HttpContext httpContext)
        {
            var cookie = cookieStorage.ReadAdmin(httpContext.Request);
            return cookie.Match(some =>
            {
                if (!adminSettings.IsValid(some).HasValue)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }, () => false);
        }
    }

}
