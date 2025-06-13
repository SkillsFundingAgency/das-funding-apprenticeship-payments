namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

public class ReleasePaymentsForProviderInput : InputBase
{
    public ReleasePaymentsForProviderInput(CollectionDetails collectionDetails, long ukprn, string instanceId) : base(instanceId)
    {
        CollectionDetails = collectionDetails;
        Ukprn = ukprn;
    }

    public CollectionDetails CollectionDetails { get; }
    public long Ukprn { get; }
}