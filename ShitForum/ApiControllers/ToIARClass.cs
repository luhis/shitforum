using System;
using System.Threading.Tasks;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ShitForum.ApiControllers
{

    public static class ToIARClass
    {
        public static IActionResult ToIAR(this IActionResult a) => a;
    }

}
