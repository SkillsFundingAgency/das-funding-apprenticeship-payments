namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

public class ApplyFreezeAndUnfreezeInput : InputBase
{
    public CollectionDetails CollectionDetails { get; }
    public Guid ApprenticeshipKey { get; }

    public ApplyFreezeAndUnfreezeInput(CollectionDetails collectionDetails, Guid apprenticeshipKey, string instanceId) : base(instanceId)
    {
        CollectionDetails = collectionDetails;
        ApprenticeshipKey = apprenticeshipKey;
    }
}