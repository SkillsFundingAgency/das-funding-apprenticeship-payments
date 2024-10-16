namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetDuePayments
{
    public class GetDuePaymentsQuery
    {
        public GetDuePaymentsQuery(Guid apprenticeshipKey, short collectionYear, byte collectionPeriod)
        {
            ApprenticeshipKey = apprenticeshipKey;
            CollectionPeriod = collectionPeriod;
            CollectionYear = collectionYear;
        }

        public Guid ApprenticeshipKey { get; }
        public byte CollectionPeriod { get; }
        public short CollectionYear { get; set; }
    }
}
