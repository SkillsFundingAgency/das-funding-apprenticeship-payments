namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.SetLearnerReference;

public class SetLearnerReferenceCommand
{
    public SetLearnerReferenceCommand(Guid apprenticeshipKey, string learnerReference)
    {
        ApprenticeshipKey = apprenticeshipKey;
        LearnerReference = learnerReference;
    }

    public Guid ApprenticeshipKey { get; }
    public string LearnerReference { get; }
}