namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ReleasePayment
{
    public class ReleasePaymentCommand
    {
        public ReleasePaymentCommand(Guid apprenticeshipKey, Guid paymentKey)
        {
            PaymentKey = paymentKey;
            ApprenticeshipKey = apprenticeshipKey;
        }

        public Guid PaymentKey { get; }
        public Guid ApprenticeshipKey { get; }
    }
}
