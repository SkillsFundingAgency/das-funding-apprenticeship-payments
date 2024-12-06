namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnfreezePayments;

public class UnfreezePaymentsCommand
{
    public UnfreezePaymentsCommand(Guid apprenticeshipKey)
    {
        ApprenticeshipKey = apprenticeshipKey;
    }

    public Guid ApprenticeshipKey { get; }
}