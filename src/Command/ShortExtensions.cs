namespace SFA.DAS.Funding.ApprenticeshipPayments.Command;

public static class ShortExtensions
{
    public static short ToStartingCalendarYear(this short academicYear)
    {
        return short.Parse($"20{short.Parse(academicYear.ToString()[..2])}");
    }
}