using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command;

public interface ICalculatedOnProgrammeFundingEventBuilder
{
    public CalculatedOnProgrammeFundingEvent Build(PaymentEntityModel payment, Guid apprenticeshipKey);
}

public class CalculatedOnProgrammeFundingEventBuilder : ICalculatedOnProgrammeFundingEventBuilder
{
    public CalculatedOnProgrammeFundingEvent Build(PaymentEntityModel payment, Guid apprenticeshipKey)
    {
        return new CalculatedOnProgrammeFundingEvent
        {
            CollectionYear = payment.PaymentYear,
            CollectionMonth = payment.PaymentPeriod,
            Amount = payment.Amount,
            ApprenticeshipKey = apprenticeshipKey
        };
    }
}