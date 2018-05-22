using Microsoft.AspNetCore.Http;

namespace ShitForum.IsAdmin
{
    public interface IIsAdmin
    {
        bool IsAdmin(HttpContext httpContext);
    }
}
