namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

public class ApplyFreezeAndUnfreezeInput : InputBase
{
    public CollectionDetails CollectionDetails { get; }
    public Guid ApprenticeshipKey { get; }

    public ApplyFreezeAndUnfreezeInput(CollectionDetails collectionDetails, Guid apprenticeshipKey, string orchestrationInstanceId) : base(orchestrationInstanceId)
    {
        CollectionDetails = collectionDetails;
        ApprenticeshipKey = apprenticeshipKey;
    }
}