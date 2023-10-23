using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;

public static class PeriodHelper
{
    public static void SetDeliveryPeriodsAccordingToCalendarMonths(this EarningsGeneratedEvent earningsGeneratedEvent)
    {
        foreach (var deliveryPeriod in earningsGeneratedEvent.DeliveryPeriods)
        {
            deliveryPeriod.AcademicYear = deliveryPeriod.CalenderYear.ToAcademicYear(deliveryPeriod.CalendarMonth);
            deliveryPeriod.Period = deliveryPeriod.CalendarMonth.ToDeliveryPeriod();
        }
    }

    public static void SetDeliveryPeriodsAccordingToCalendarMonths(this ApprenticeshipEarningsRecalculatedEvent earningsRecalculatedEvent)
    {
        foreach (var deliveryPeriod in earningsRecalculatedEvent.DeliveryPeriods)
        {
            deliveryPeriod.AcademicYear = deliveryPeriod.CalenderYear.ToAcademicYear(deliveryPeriod.CalendarMonth);
            deliveryPeriod.Period = deliveryPeriod.CalendarMonth.ToDeliveryPeriod();
        }
    }

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
}