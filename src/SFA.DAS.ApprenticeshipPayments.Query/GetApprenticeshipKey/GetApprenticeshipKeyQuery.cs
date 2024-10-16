namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetApprenticeshipKey
{
    public class GetApprenticeshipKeyQuery
    {
        public long Ukprn { get; }
        public long Uln { get; }

        public GetApprenticeshipKeyQuery(long ukprn, long uln)
        {
            Ukprn = ukprn;
            Uln = uln;
        }
    }
}
