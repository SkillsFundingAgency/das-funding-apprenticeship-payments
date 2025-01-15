using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

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

    public async Task<IEnumerable<long>> GetAllProviders()
    {
        return await DbContext.Apprenticeships.Select(x => x.Ukprn).Distinct().ToListAsync();
    }

    public async Task<Guid?> GetApprenticeshipKey(long ukprn, long uln)
    {
        var apprenticeshipKey = await DbContext.Apprenticeships.Where(x => x.Ukprn == ukprn && x.Uln == uln).Select(x => x.ApprenticeshipKey).SingleOrDefaultAsync();
        if (apprenticeshipKey == Guid.Empty)
        {
            return null;
        }
        return apprenticeshipKey;
    }
}