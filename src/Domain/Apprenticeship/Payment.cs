namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship
{
    public class Payment
    {
        internal Payment(short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionPeriod)
        {
            AcademicYear = academicYear;
            DeliveryPeriod = deliveryPeriod;
            Amount = amount;
            PaymentYear = collectionYear;
            PaymentPeriod = collectionPeriod;
            SentForPayment = false;
        }

        public short AcademicYear { get; }
        public byte DeliveryPeriod { get; }
        public decimal Amount { get; }
        public short PaymentYear { get; }
        public byte PaymentPeriod { get; }
        public bool SentForPayment { get; }
    }
}
