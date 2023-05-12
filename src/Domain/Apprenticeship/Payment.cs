namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship
{
    public class Payment
    {
        internal Payment(short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionPeriod)
        {
            AcademicYear = academicYear;
            DeliveryPeriod = deliveryPeriod;
            Amount = amount;
            CollectionYear = collectionYear;
            CollectionPeriod = collectionPeriod;
            SentForPayment = false;
        }

        public short AcademicYear { get; }
        public byte DeliveryPeriod { get; }
        public decimal Amount { get; }
        public short CollectionYear { get; }
        public byte CollectionPeriod { get; }
        public bool SentForPayment { get; }
    }
}
