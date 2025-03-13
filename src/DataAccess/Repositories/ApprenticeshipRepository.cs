using Microsoft.EntityFrameworkCore;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

public class ApprenticeshipRepository : IApprenticeshipRepository
{
    private readonly Lazy<ApprenticeshipPaymentsDataContext> _lazyContext;
    private readonly IDasServiceBusEndpoint _busEndpoint;
    private ApprenticeshipPaymentsDataContext DbContext => _lazyContext.Value;

    public ApprenticeshipRepository(Lazy<ApprenticeshipPaymentsDataContext> dbContext, IDasServiceBusEndpoint busEndpoint)
    {
        _busEndpoint = busEndpoint;
        _lazyContext = dbContext;
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
        await using var transaction = await DbContext.Database.BeginTransactionAsync();

        await DbContext.SaveChangesAsync();

        foreach (dynamic domainEvent in apprenticeship.FlushEvents())
        {
            await _busEndpoint.Publish(domainEvent);
        }

        await transaction.CommitAsync();
    }

    public async Task<bool> Exists(Guid key)
    {
        var apprenticeship = await DbContext.Apprenticeships
            .SingleOrDefaultAsync(x => x.ApprenticeshipKey == key);

        return apprenticeship != null;
    }
}