namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

public class GetLearnersInIlrSubmissionInput : InputBase
{
    public long Ukprn { get; }
    public short AcademicYear { get; }

    public GetLearnersInIlrSubmissionInput(long ukprn, short academicYear, string orchestrationInstanceId) : base(orchestrationInstanceId)
    {
        Ukprn = ukprn;
        AcademicYear = academicYear;
    }
}