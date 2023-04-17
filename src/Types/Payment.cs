namespace SFA.DAS.Funding.ApprenticeshipPayments.Types;

public class Payment
{
    public short AcademicYear { get; set; }
    public byte DeliveryPeriod { get; set; }
    public decimal Amount { get; set; }
    public short PaymentYear { get; set; }
    public byte PaymentPeriod { get; set; }
}