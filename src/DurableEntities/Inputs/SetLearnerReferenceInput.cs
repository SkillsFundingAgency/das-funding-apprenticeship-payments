namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs
{
    public class SetLearnerReferenceInput
    {
        public Guid ApprenticeshipKey { get; }
        public string LearnerReference { get; }

        public SetLearnerReferenceInput(Guid apprenticeshipKey, string learnerReference)
        {
            ApprenticeshipKey = apprenticeshipKey;
            LearnerReference = learnerReference;
        }
    }
}
