using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
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

        public Apprenticeship.Apprenticeship LoadExisting(ApprenticeshipEntityModel entityModel)
        {
            return new Apprenticeship.Apprenticeship(entityModel.ApprenticeshipKey, entityModel.Earnings.Select(e => new Earning(
                e.AcademicYear,
                e.DeliveryPeriod,
                e.Amount,
                e.CollectionYear,
                e.CollectionMonth,
                e.FundingLineType
            )).ToList(), entityModel.Payments.Select(p => new Payment(
                p.AcademicYear,
                p.DeliveryPeriod,
                p.Amount,
                p.CollectionYear,
                p.CollectionPeriod,
                p.FundingLineType
            )).ToList());
        }
    }
}
