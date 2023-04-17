namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship
{
    public class Payment
    {
        internal Payment(short academicYear, byte deliveryPeriod, decimal amount, short paymentYear, byte paymentPeriod)
        {
            AcademicYear = academicYear;
            DeliveryPeriod = deliveryPeriod;
            Amount = amount;
            PaymentYear = paymentYear;
            PaymentPeriod = paymentPeriod;
        }

        public short AcademicYear { get; }
        public byte DeliveryPeriod { get; }
        public decimal Amount { get; }
        public short PaymentYear { get; }
        public byte PaymentPeriod { get; }
    }
}
