using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories
{
    public class ApprenticeshipQueryRepository : IApprenticeshipQueryRepository
    {
        private readonly Lazy<ApprenticeshipPaymentsDataContext> _lazyContext;
        private ApprenticeshipPaymentsDataContext DbContext => _lazyContext.Value;

        public ApprenticeshipQueryRepository(Lazy<ApprenticeshipPaymentsDataContext> dbContext)
        {
            _lazyContext = dbContext;
        }
        public async Task<IEnumerable<Guid>> GetWithDuePayments(short collectionYear, byte collectionPeriod)
        {
            return await DbContext.Apprenticeships.Where(x => x.Payments.Any(payment => (payment.CollectionYear == collectionYear && payment.CollectionPeriod <= collectionPeriod) || payment.CollectionYear < collectionYear)).Select(x => x.ApprenticeshipKey).ToListAsync();
        }
    }
}
