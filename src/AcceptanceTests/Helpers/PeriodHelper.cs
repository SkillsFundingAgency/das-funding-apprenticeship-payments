using SFA.DAS.Funding.ApprenticeshipEarnings.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;

public static class PeriodHelper
{
    public static byte ToDeliveryPeriod(this byte calendarMonth)
    {
        if (calendarMonth < 8)
            return (byte)(calendarMonth + 5);

        return (byte)(calendarMonth - 7);
    }

    public static short ToAcademicYear(this short calendarYear, byte calendarMonth)
    {
        var calendarYearTwoDigit = short.Parse(calendarYear.ToString().Substring(2, 2));
        if (calendarMonth < 8)
            return short.Parse($"{calendarYearTwoDigit - 1}{calendarYearTwoDigit}");

        return short.Parse($"{calendarYearTwoDigit}{calendarYearTwoDigit + 1}");
    }

    public static DeliveryPeriod CreateDeliveryPeriod(byte calendarMonth, short calendarYear, decimal learningAmount)
    {
        return new DeliveryPeriod(
            calendarMonth,
            calendarYear, 
            calendarMonth.ToDeliveryPeriod(), 
            calendarYear.ToAcademicYear(calendarMonth), 
            learningAmount, 
            "fundingLineType");
    }
}