namespace SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

public interface IApprenticeshipQueryRepository
{
    Task<IEnumerable<Guid>> GetWithDuePayments(short collectionYear, byte collectionPeriod);
    Task<IEnumerable<long>> GetAllProviders();
    Task<Guid?> GetApprenticeshipKey(long ukprn, long uln);
}