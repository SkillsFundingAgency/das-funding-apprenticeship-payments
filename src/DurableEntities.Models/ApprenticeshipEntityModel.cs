using SFA.DAS.Funding.ApprenticeshipEarnings.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models
{
    public class ApprenticeshipEntityModel
    {
        public Guid ApprenticeshipKey { get; set; }
        public List<EarningEntityModel> Earnings { get; set; } = new();
        public List<PaymentEntityModel> Payments { get; set; } = new();
        public long FundingEmployerAccountId { get; set; }
        public EmployerType EmployerType { get; set; }
        public long FundingCommitmentId { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public long Uln { get; set; }
        public long Ukprn { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public string? CourseCode { get; set; }
        public DateTime StartDate { get; set; }
        public long ApprovalsApprenticeshipId { get; set; }
    }
}
