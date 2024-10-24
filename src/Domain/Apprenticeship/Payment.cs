using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

[Table("Payment", Schema = "Domain")]
public class Payment
{
    private Payment() { }

    public Payment(Guid apprenticeshipKey, short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionPeriod, string fundingLineType, Guid earningsProfileId)
    {
        Key = Guid.NewGuid();
        ApprenticeshipKey = apprenticeshipKey;
        AcademicYear = academicYear;
        DeliveryPeriod = deliveryPeriod;
        Amount = amount;
        CollectionYear = collectionYear;
        CollectionPeriod = collectionPeriod;
        FundingLineType = fundingLineType;
        SentForPayment = false;
        EarningsProfileId = earningsProfileId;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Key { get; private set; }
    public Guid ApprenticeshipKey { get; private set; }
    public short AcademicYear { get; private set; }
    public decimal Amount { get; private set; }
    public byte CollectionPeriod { get; private set; }
    public short CollectionYear { get; private set; }
    public byte DeliveryPeriod { get; private set; }
    public string FundingLineType { get; private set; }
    public bool SentForPayment { get; private set; }
    public Guid EarningsProfileId { get; private set; }
    public bool NotPaidDueToFreeze { get; private set; }

    public void MarkAsNotPaid()
    {
        NotPaidDueToFreeze = true;
    }

    public void Unfreeze(short collectionYear, byte collectionPeriod)
    {
        NotPaidDueToFreeze = false;
        CollectionYear = collectionYear;
        CollectionPeriod = collectionPeriod;
    }

    public void MarkAsSent()
    {
        SentForPayment = true;
    }
}