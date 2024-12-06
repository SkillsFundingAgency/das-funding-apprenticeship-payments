using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Types;

public class FinalisedOnProgammeLearningPaymentEvent : IDomainEvent
{
    public DateTime? ActualEndDate { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public byte CollectionPeriod { get; set; }
    public short CollectionYear { get; set; }
    public decimal Amount { get; set; }
    public EmployerType ApprenticeshipEmployerType { get; set; }
    public EmployerDetails EmployerDetails { get; set; }
    public ApprenticeshipEarning ApprenticeshipEarning { get; set; }
    public Apprenticeship Apprenticeship { get; set; }
    public string? CourseCode { get; set; }
    public string FundingLineType { get; set; }
    public Guid EarningsProfileId { get; set; }
}