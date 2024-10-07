namespace SFA.DAS.ApprenticeshipPayments.Query.GetApprenticeshipsWithDuePayments
{
    public class GetApprenticeshipsWithDuePaymentsQuery
    {
        public GetApprenticeshipsWithDuePaymentsQuery(byte collectionPeriod, short collectionYear)
        {
            CollectionPeriod = collectionPeriod;
            CollectionYear = collectionYear;
        }

        public byte CollectionPeriod { get; }
        public short CollectionYear { get; set; }
    }
}
