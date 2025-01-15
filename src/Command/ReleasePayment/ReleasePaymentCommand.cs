namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ReleasePayment;

public class ReleasePaymentCommand
{
    public ReleasePaymentCommand(Guid apprenticeshipKey, Guid paymentKey, short collectionYear, byte collectionPeriod)
    {
        PaymentKey = paymentKey;
        CollectionPeriod = collectionPeriod;
        CollectionYear = collectionYear;
        ApprenticeshipKey = apprenticeshipKey;
    }

    public short CollectionYear { get; set; }
    public byte CollectionPeriod { get; }
    public Guid PaymentKey { get; }
    public Guid ApprenticeshipKey { get; }
}