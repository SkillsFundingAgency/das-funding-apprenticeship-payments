namespace SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

public interface IApprenticeshipRepository
{
    Task Add(Domain.Apprenticeship.Apprenticeship apprenticeship);
    Task<Domain.Apprenticeship.Apprenticeship> Get(Guid key);
    Task Update(Domain.Apprenticeship.Apprenticeship apprenticeship);
}