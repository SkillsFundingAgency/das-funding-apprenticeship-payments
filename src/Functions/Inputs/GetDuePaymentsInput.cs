namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

public class GetDuePaymentsInput
{
    public CollectionDetails CollectionDetails { get; }
    public Guid ApprenticeshipKey { get; }

    public GetDuePaymentsInput(CollectionDetails collectionDetails, Guid apprenticeshipKey)
    {
        CollectionDetails = collectionDetails;
        ApprenticeshipKey = apprenticeshipKey;
    }
}