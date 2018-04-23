using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShitForum
{
    public static class ToIARClass
    {
        public static IActionResult ToIAR(this IActionResult a) => a;
        public static Task<IActionResult> ToIART(this IActionResult a) => Task.FromResult(a);
    }
}
