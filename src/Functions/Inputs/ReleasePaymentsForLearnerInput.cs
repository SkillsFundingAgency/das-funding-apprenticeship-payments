using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Dtos;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

public class ReleasePaymentsForLearnerInput :InputBase
{
    public ReleasePaymentsForLearnerInput(CollectionDetails collectionDetails, Learner learner, string instanceId) : base(instanceId)
    {
        CollectionDetails = collectionDetails;
        Learner = learner;
    }

    public CollectionDetails CollectionDetails { get; }
    public Learner Learner { get; }
}