using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Services.Interfaces
{
    public interface IAnalyticsService
    {
        [Pure]
        Task<IReadOnlyList<AnalyticsReport>> GetHits(CancellationToken cancellationToken);

        [Pure]
        Task PurgeOldData(CancellationToken cancellationToken);

        [Pure]
        Task Add(AnalyticsReport rpt, CancellationToken cancellationToken);
    }
}
