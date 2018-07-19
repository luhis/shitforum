using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using ShitForum.Attributes;
using ShitForum.Models;

namespace ShitForum.Pages
{
    [CookieAuth]
    public class AnalyticsModel : PageModel
    {
        private readonly IAnalyticsService analyticsService;

        public AnalyticsModel(IAnalyticsService analyticsService)
        {
            this.analyticsService = analyticsService;
        }

        public AnalyticsViewModel Model { get; private set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            this.Model = Map(await this.analyticsService.GetHits(cancellationToken));
        }

        private static AnalyticsViewModel Map(IReadOnlyList<AnalyticsReport> getHits)
        {
            var grouped = getHits.GroupBy(a => a.Location).Select(a => new LocationDetails(a.Key, a.Count(), a.GroupBy(b => b.ThumbPrint).Count())).ToList();
            return new AnalyticsViewModel(grouped);
        }
    }
}
