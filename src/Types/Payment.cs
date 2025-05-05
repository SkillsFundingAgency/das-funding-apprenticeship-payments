namespace SFA.DAS.Funding.ApprenticeshipPayments.Types;

public class Payment
{
    public short AcademicYear { get; set; }
    public byte DeliveryPeriod { get; set; }
    public decimal Amount { get; set; }
    public short CollectionYear { get; set; }
    public byte CollectionPeriod { get; set; }
    public string FundingLineType { get; set; }
    public Guid EarningsProfileId { get; set; }
    public string? PaymentType { get; set; }
}