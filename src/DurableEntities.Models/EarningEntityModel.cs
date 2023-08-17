namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models
{
    public class EarningEntityModel
    {
        public byte DeliveryPeriod { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionMonth { get; set; }
        public short CollectionYear { get; set; }
        public decimal Amount { get; set; }
        public string FundingLineType { get; set; }
    }
}
