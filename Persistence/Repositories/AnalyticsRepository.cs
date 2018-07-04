using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;

namespace Persistence.Repositories
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly ForumContext client;

        public AnalyticsRepository(ForumContext client)
        {
            this.client = client;
        }

        Task IAnalyticsRepository.Add(AnalyticsReport analyticsReport, CancellationToken cancellationToken)
        {
            this.client.AnalyticsReports.Add(analyticsReport);
            return this.client.SaveChangesAsync(cancellationToken);
        }

        Task<IReadOnlyList<AnalyticsReport>> IAnalyticsRepository.GetAll(CancellationToken cancellationToken)
        {
            return this.client.AnalyticsReports.ToReadOnlyAsync(cancellationToken);
        }
    }
}
