namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetLearnersInILR
{
    public class GetLearnersInILRQuery
    {
        public long Ukprn { get; }
        public short AcademicYear { get; }

        public GetLearnersInILRQuery(long ukprn, short academicYear)
        {
            Ukprn = ukprn;
            AcademicYear = academicYear;
        }
    }
}
