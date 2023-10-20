using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Factories
{
    public interface IApprenticeshipFactory
    {
        Apprenticeship.Apprenticeship CreateNew(ApprenticeshipEntityModel entityModel);
        Apprenticeship.Apprenticeship LoadExisting(ApprenticeshipEntityModel entityModel);
    }
}
