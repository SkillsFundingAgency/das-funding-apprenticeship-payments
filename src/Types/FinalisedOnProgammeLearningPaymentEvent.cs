using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Types;

public class FinalisedOnProgammeLearningPaymentEvent
{
    public long AccountId { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public byte CollectionMonth { get; set; }
    public short CollectionYear { get; set; }
    public decimal Amount { get; set; }
    public FundingType FundingType { get; set; } // Confirm if FinalisedOnProgammeLearningPayment.FundingType = EmployingApprenticeshipServiceAccount then 1
    public EmployerDetails EmployerDetails { get; set; }
    public ApprenticeshipEarning ApprenticeshipEarnings { get; set; }
    public Apprenticeship Apprenticeship { get; set; }
    public int CourseCode { get; set; }
}