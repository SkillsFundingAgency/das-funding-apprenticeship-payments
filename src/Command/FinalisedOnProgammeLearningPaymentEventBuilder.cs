using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command;

public interface IFinalisedOnProgammeLearningPaymentEventBuilder
{
    public FinalisedOnProgammeLearningPaymentEvent Build(PaymentEntityModel payment, Guid apprenticeshipKey);
}

public class FinalisedOnProgammeLearningPaymentEventBuilder : IFinalisedOnProgammeLearningPaymentEventBuilder
{
    public FinalisedOnProgammeLearningPaymentEvent Build(PaymentEntityModel payment, Guid apprenticeshipKey)
    {
        return new FinalisedOnProgammeLearningPaymentEvent
        {
            CollectionYear = payment.PaymentYear,
            CollectionMonth = payment.PaymentPeriod,
            Amount = payment.Amount,
            ApprenticeshipKey = apprenticeshipKey
        };
    }
}