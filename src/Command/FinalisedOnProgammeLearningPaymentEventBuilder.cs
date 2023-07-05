using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command;

public interface IFinalisedOnProgammeLearningPaymentEventBuilder
{
    public FinalisedOnProgammeLearningPaymentEvent Build(PaymentEntityModel payment, Guid apprenticeshipKey, short totalNumberOfPayments);
}

public class FinalisedOnProgammeLearningPaymentEventBuilder : IFinalisedOnProgammeLearningPaymentEventBuilder
{
    public FinalisedOnProgammeLearningPaymentEvent Build(PaymentEntityModel payment, Guid apprenticeshipKey, short totalNumberOfPayments)
    {
        return new FinalisedOnProgammeLearningPaymentEvent        
        {
            CollectionYear = payment.CollectionYear,
            CollectionMonth = payment.CollectionPeriod,
            Amount = payment.Amount,
            ApprenticeshipKey = apprenticeshipKey,
            ApprenticeshipEarnings = new ApprenticeshipEarning
            {
                GovernmentContributionPercentage = 0.95m,
                ApprenticeshipEarningsId = Guid.NewGuid(),
                NumberOfInstalments = totalNumberOfPayments
            }
        };
    }
}