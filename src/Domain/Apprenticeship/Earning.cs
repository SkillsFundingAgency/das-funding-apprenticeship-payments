using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship
{
    [Table("Earning", Schema = "Domain")]
    public class Earning
    {
        public Earning(Guid key, short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionMonth, string fundingLineType, Guid earningsProfileId)
        {
            Key = key;
            AcademicYear = academicYear;
            DeliveryPeriod = deliveryPeriod;
            Amount = amount;
            CollectionMonth = collectionMonth;
            FundingLineType = fundingLineType;
            CollectionYear = collectionYear;
            EarningsProfileId = earningsProfileId;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Key { get; }
        public short AcademicYear { get; }
        public decimal Amount { get; }
        public byte CollectionMonth { get; }
        public short CollectionYear { get; }
        public byte DeliveryPeriod { get; }
        public string FundingLineType { get; }
        public Guid EarningsProfileId { get; }
    }
}
