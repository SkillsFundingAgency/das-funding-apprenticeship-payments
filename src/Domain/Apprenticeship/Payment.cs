using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship
{
    [Table("Payment", Schema = "Domain")]
    public class Payment
    {
        public Payment(short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionPeriod, string fundingLineType, Guid earningsProfileId)
        {
            Key = Guid.NewGuid();
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
        public Guid Key { get; }
        public short AcademicYear { get; }
        public decimal Amount { get; }
        public byte CollectionPeriod { get; }
        public short CollectionYear { get; }
        public byte DeliveryPeriod { get; }
        public string FundingLineType { get; }
        public bool SentForPayment { get; }
        public Guid EarningsProfileId { get; }
        public bool NotPaidDueToFreeze { get; }
    }
}
