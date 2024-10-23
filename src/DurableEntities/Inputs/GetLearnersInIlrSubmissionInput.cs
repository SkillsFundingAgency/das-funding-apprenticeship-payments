namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs
{
    public class GetLearnersInIlrSubmissionInput
    {
        public long Ukprn { get; }
        public short AcademicYear { get; }

        public GetLearnersInIlrSubmissionInput(long ukprn, short academicYear)
        {
            Ukprn = ukprn;
            AcademicYear = academicYear;
        }
    }
}
