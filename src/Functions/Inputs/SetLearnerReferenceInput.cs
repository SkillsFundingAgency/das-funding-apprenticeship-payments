namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

public class SetLearnerReferenceInput : InputBase
{
    public Guid ApprenticeshipKey { get; }
    public string LearnerReference { get; }

    public SetLearnerReferenceInput(Guid apprenticeshipKey, string learnerReference, string orchestrationInstanceId) : base(orchestrationInstanceId)
    {
        ApprenticeshipKey = apprenticeshipKey;
        LearnerReference = learnerReference;
    }
}