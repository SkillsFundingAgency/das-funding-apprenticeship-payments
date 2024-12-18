using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;

public class RecalculateApprenticeshipPaymentsCommand
{
    public RecalculateApprenticeshipPaymentsCommand(Guid apprenticeshipKey, List<Earning> newEarnings, DateTime startDate, DateTime plannedEndDate, int ageAtStartOfApprenticeship)
    {
        ApprenticeshipKey = apprenticeshipKey;
        NewEarnings = newEarnings;
        StartDate = startDate;
        PlannedEndDate = plannedEndDate;
        AgeAtStartOfApprenticeship = ageAtStartOfApprenticeship;
    }

    public Guid ApprenticeshipKey { get; }
    public List<Earning> NewEarnings { get; set; }
    public DateTime StartDate { get; }
    public DateTime PlannedEndDate { get; }
    public int AgeAtStartOfApprenticeship { get; }
}