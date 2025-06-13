namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

public class GetDuePaymentsInput : InputBase
{
    public CollectionDetails CollectionDetails { get; }
    public Guid ApprenticeshipKey { get; }

    public GetDuePaymentsInput(CollectionDetails collectionDetails, Guid apprenticeshipKey, string instanceId) : base(instanceId)
    {
        CollectionDetails = collectionDetails;
        ApprenticeshipKey = apprenticeshipKey;
    }
}