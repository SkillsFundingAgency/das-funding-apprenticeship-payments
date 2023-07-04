namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models
{
    public class ApprenticeshipEntityModel
    {
        public Guid ApprenticeshipKey { get; set; }
        public List<EarningEntityModel> Earnings { get; set; }
        public List<PaymentEntityModel> Payments { get; set; }
        public long Uln { get; set; }
        public DateTime StartDate { get; set; }
        public long Ukprn { get; set; }
    }
}
