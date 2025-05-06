namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetDuePayments;

public class GetDuePaymentsQuery
{
    public GetDuePaymentsQuery(Guid apprenticeshipKey, short collectionYear, byte collectionPeriod, string paymentType)
    {
        ApprenticeshipKey = apprenticeshipKey;
        CollectionPeriod = collectionPeriod;
        CollectionYear = collectionYear;
        PaymentType = paymentType;
    }

    public Guid ApprenticeshipKey { get; }
    public byte CollectionPeriod { get; }
    public short CollectionYear { get; set; }
    public string PaymentType { get; set; } = string.Empty;
}