namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs
{
    public class ApplyFreezeAndUnfreezeInput
    {
        public CollectionDetails CollectionDetails { get; }
        public Guid ApprenticeshipKey { get; }

        public ApplyFreezeAndUnfreezeInput(CollectionDetails collectionDetails, Guid apprenticeshipKey)
        {
            CollectionDetails = collectionDetails;
            ApprenticeshipKey = apprenticeshipKey;
        }
    }
}
