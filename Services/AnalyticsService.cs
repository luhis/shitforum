using Domain;
using Domain.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Services.Interfaces;

namespace Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepository analyticsRepository;

        public AnalyticsService(IAnalyticsRepository analyticsRepository)
        {
            this.analyticsRepository = analyticsRepository;
        }

        Task IAnalyticsService.Add(AnalyticsReport rpt, CancellationToken cancellationToken)
        {
            return this.analyticsRepository.Add(rpt, cancellationToken);
        }

        Task<IReadOnlyList<AnalyticsReport>> IAnalyticsService.GetHits(CancellationToken cancellationToken)
        {
            return this.analyticsRepository.GetAll(cancellationToken);
        }

        Task IAnalyticsService.PurgeOldData(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
