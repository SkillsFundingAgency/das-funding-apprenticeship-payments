namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;

public static class AcademicYearHelper
{
    private static readonly Random Random = new Random();

    public static short GetRandomValidAcademicYear()
    {
        var startYearCode = Random.Next(10, 98);
        return short.Parse($"{startYearCode}{startYearCode+1}");
    }
}