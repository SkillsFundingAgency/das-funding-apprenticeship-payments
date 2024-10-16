using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Dtos;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs
{
    public class ReleasePaymentsForLearnerInput
    {
        public ReleasePaymentsForLearnerInput(CollectionDetails collectionDetails, Learner learner)
        {
            CollectionDetails = collectionDetails;
            Learner = learner;
        }

        public CollectionDetails CollectionDetails { get; }
        public Learner Learner { get; }
    }
}
