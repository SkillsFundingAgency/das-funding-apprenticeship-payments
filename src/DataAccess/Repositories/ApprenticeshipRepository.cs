using Microsoft.EntityFrameworkCore;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

public class ApprenticeshipRepository : IApprenticeshipRepository
{
    private readonly Lazy<ApprenticeshipPaymentsDataContext> _lazyContext;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private ApprenticeshipPaymentsDataContext DbContext => _lazyContext.Value;

    public ApprenticeshipRepository(Lazy<ApprenticeshipPaymentsDataContext> dbContext, IDomainEventDispatcher domainEventDispatcher)
    {
        _lazyContext = dbContext;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task Add(Domain.Apprenticeship.IApprenticeship apprenticeship)
    {
        await DbContext.AddAsync(apprenticeship);
        await DbContext.SaveChangesAsync();
    }

    public async Task<Domain.Apprenticeship.IApprenticeship> Get(Guid key)
    {
        var apprenticeship = await DbContext.Apprenticeships
            .Include(x => x.Earnings)
            .Include(x => x.Payments)
            .SingleAsync(x => x.ApprenticeshipKey == key);

        return apprenticeship;
    }

    public async Task Update(Domain.Apprenticeship.IApprenticeship apprenticeship)
    {
        await DbContext.SaveChangesAsync();
    }
}