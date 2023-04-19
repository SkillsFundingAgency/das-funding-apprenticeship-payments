namespace SFA.DAS.Funding.ApprenticeshipPayments.Types;

public class PaymentsGeneratedEvent
{
    public Guid ApprenticeshipKey { get; set; }
    public List<Payment> Payments { get; set; }
}