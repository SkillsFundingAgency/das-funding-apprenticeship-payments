using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

public interface IApprenticeshipRepository
{
    Task Add(IApprenticeship apprenticeship);
    Task<IApprenticeship> Get(Guid key);
    Task Update(IApprenticeship apprenticeship);
    Task<bool> Exists(Guid key);
}