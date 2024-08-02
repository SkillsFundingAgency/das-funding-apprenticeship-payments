﻿using SFA.DAS.Funding.ApprenticeshipPayments.Domain;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests;

public class TestSystemClock : ISystemClockService
{
    private static DateTime _testTime;

    public DateTimeOffset UtcNow => _testTime;

    public DateTime Now => _testTime;

    public static void SetDateTime(DateTime dateTime)
    {
        _testTime = dateTime;
    }

    public static ISystemClockService Instance()
    {
        return new TestSystemClock();
    }
}