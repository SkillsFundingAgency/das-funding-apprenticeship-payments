using SFA.DAS.Funding.ApprenticeshipEarnings.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Types;
#pragma warning disable CS8618

public class FinalisedOnProgammeLearningPaymentEvent
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
}

#pragma warning enable CS8618
