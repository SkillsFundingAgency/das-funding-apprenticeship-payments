using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command;

public interface IPaymentsGeneratedEventBuilder
{
    PaymentsGeneratedEvent Build(Apprenticeship apprenticeship);
}

public class PaymentsGeneratedEventBuilder : IPaymentsGeneratedEventBuilder
{
    public PaymentsGeneratedEvent Build(Apprenticeship apprenticeship)
    {
        return new PaymentsGeneratedEvent
        {
            ApprenticeshipKey = apprenticeship.ApprenticeshipKey,
            Payments = apprenticeship.Payments.Select(x => new ApprenticeshipPayments.Types.Payment
            {
                AcademicYear = x.AcademicYear,
                Amount = x.Amount,
                DeliveryPeriod = x.DeliveryPeriod,
                PaymentPeriod = x.PaymentPeriod,
                PaymentYear = x.PaymentYear
            }).ToList()
        };
    }
}