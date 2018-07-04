using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IAnalyticsRepository
    {
        [Pure]
        Task Add(AnalyticsReport analyticsReport, CancellationToken cancellationToken);

        [Pure]
        Task<IReadOnlyList<AnalyticsReport>> GetAll(CancellationToken cancellationToken);
    }
}
