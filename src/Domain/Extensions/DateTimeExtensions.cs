namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Extensions;

public static class DateTimeExtensions
{
    public static short ToAcademicYear(this DateTime dateTime)
    {
        var twoDigitYear = short.Parse(dateTime.Year.ToString().Substring(2));

        if (dateTime.Month >= 8)
            return short.Parse($"{twoDigitYear}{twoDigitYear + 1}");

        return short.Parse($"{twoDigitYear - 1}{twoDigitYear}");
    }
}
