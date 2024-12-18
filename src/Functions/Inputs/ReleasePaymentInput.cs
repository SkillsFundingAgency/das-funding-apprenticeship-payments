namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

public class ReleasePaymentInput
{
    public ReleasePaymentInput(Guid apprenticeshipKey, Guid paymentKey)
    {
        ApprenticeshipKey = apprenticeshipKey;
        PaymentKey = paymentKey;
    }

    public Guid ApprenticeshipKey { get; }
    public Guid PaymentKey { get; }
}