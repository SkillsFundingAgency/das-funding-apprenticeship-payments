﻿namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Extensions;

public static class DateTimeExtensions
{
    public static short ToAcademicYear(this DateTime dateTime)
    {
        var twoDigitYear = short.Parse(dateTime.Year.ToString().Substring(2));

        if (dateTime.Month >= 8)
            return short.Parse($"{twoDigitYear}{twoDigitYear + 1}");

        return short.Parse($"{twoDigitYear - 1}{twoDigitYear}");
    }

    public static byte ToDeliveryPeriod(this DateTime dateTime)
    {
        if (dateTime.Month >= 8)
            return (byte)(dateTime.Month - 7);
        else
            return (byte)(dateTime.Month + 5);
    }

    public static short ToCalendarYear(this short academicYear, byte deliveryPeriod)
    {
        if (deliveryPeriod >= 6)
            return short.Parse($"20{academicYear.ToString().Substring(2, 2)}");
        else
            return short.Parse($"20{academicYear.ToString().Substring(0, 2)}");
    }

    public static byte ToCalendarMonth(this byte deliveryPeriod)
    {
        if (deliveryPeriod >= 6)
            return (byte)(deliveryPeriod - 5);
        else
            return (byte)(deliveryPeriod + 7);
    }

    public static DateTime ToDateTime(this short academicYear, byte deliveryPeriod)
    {
        var calendarYear = academicYear.ToCalendarYear(deliveryPeriod);
        var calendarMonth = deliveryPeriod.ToCalendarMonth();
        return new DateTime(calendarYear, calendarMonth, 1);
    }
}
