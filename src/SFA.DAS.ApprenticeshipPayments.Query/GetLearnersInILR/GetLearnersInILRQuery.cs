namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetLearnersInILR
{
    public class GetLearnersInILRQuery
    {
        public long Ukprn { get; }

        public GetLearnersInILRQuery(long ukprn)
        {
            Ukprn = ukprn;
        }
    }
}
