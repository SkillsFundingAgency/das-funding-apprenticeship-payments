namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship
{
    public class Earning
    {
        public Earning(short academicYear, byte deliveryPeriod, decimal amount)
        {
            AcademicYear = academicYear;
            DeliveryPeriod = deliveryPeriod;
            Amount = amount;
        }

        public short AcademicYear { get; }
        public byte DeliveryPeriod { get; }
        public decimal Amount { get; }
    }
}
