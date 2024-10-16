namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs
{
    public class ReleasePaymentsForProviderInput
    {
        public ReleasePaymentsForProviderInput(CollectionDetails collectionDetails, long ukprn)
        {
            CollectionDetails = collectionDetails;
            Ukprn = ukprn;
        }

        public CollectionDetails CollectionDetails { get; }
        public long Ukprn { get; }
    }
}
