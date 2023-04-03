namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship
{
    public class Earning
    {
        public Earning(short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionMonth)
        {
            AcademicYear = academicYear;
            DeliveryPeriod = deliveryPeriod;
            Amount = amount;
            CollectionMonth = collectionMonth;
            CollectionYear = collectionYear;
        }

        public short AcademicYear { get; }
        public byte DeliveryPeriod { get; }
        public decimal Amount { get; }
        public byte CollectionMonth { get; }
        public short CollectionYear { get; }
    }
}
