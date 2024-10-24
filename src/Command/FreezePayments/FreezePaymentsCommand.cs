namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.FreezePayments;

public class FreezePaymentsCommand
{
    public FreezePaymentsCommand(Guid apprenticeshipKey)
    {
        ApprenticeshipKey = apprenticeshipKey;
    }

    public Guid ApprenticeshipKey { get; }
}