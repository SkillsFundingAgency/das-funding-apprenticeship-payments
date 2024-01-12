namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship
{
    public class Payment
    {
        public Payment(short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionPeriod, string fundingLineType, Guid earningsProfileId)
        {
            AcademicYear = academicYear;
            DeliveryPeriod = deliveryPeriod;
            Amount = amount;
            CollectionYear = collectionYear;
            CollectionPeriod = collectionPeriod;
            FundingLineType = fundingLineType;
            SentForPayment = false;
            EarningsProfileId = earningsProfileId;
        }

        public short AcademicYear { get; }
        public decimal Amount { get; }
        public byte CollectionPeriod { get; }
        public short CollectionYear { get; }
        public byte DeliveryPeriod { get; }
        public string FundingLineType { get; }
        public bool SentForPayment { get; set; }
        public Guid EarningsProfileId { get; set; }
    }
}
