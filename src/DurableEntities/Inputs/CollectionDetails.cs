namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs
{
    public class CollectionDetails
    {
        public byte CollectionPeriod { get; }
        public short CollectionYear { get; }

        public CollectionDetails(byte collectionPeriod, short collectionYear)
        {
            CollectionPeriod = collectionPeriod;
            CollectionYear = collectionYear;
        }
    }
}
