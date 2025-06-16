namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

public class ReleasePaymentInput : InputBase
{
    public ReleasePaymentInput(
        Guid apprenticeshipKey, 
        Guid paymentKey, 
        CollectionDetails collectionDetails, 
        string orchestrationInstanceId) :base(orchestrationInstanceId)
    {
        ApprenticeshipKey = apprenticeshipKey;
        PaymentKey = paymentKey;
        CollectionDetails = collectionDetails;
    }

    public Guid ApprenticeshipKey { get; }
    public CollectionDetails CollectionDetails { get; }
    public Guid PaymentKey { get; }
}