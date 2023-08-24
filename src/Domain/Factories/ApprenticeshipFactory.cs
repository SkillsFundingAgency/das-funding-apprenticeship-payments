using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Factories
{
    public class ApprenticeshipFactory : IApprenticeshipFactory
    {
        public Apprenticeship.Apprenticeship CreateNew(ApprenticeshipEntityModel entityModel)
        {
            var apprenticeship = new Apprenticeship.Apprenticeship(entityModel.ApprenticeshipKey);

            foreach (var earning in entityModel.Earnings)
            {
                apprenticeship.AddEarning(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, earning.CollectionYear, earning.CollectionMonth, earning.FundingLineType);
            }

            return apprenticeship;
        }
    }
}
