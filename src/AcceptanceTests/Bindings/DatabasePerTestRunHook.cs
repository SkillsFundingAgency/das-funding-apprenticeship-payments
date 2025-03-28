﻿namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings;

[Binding]
public static class DatabasePerTestRunHook
{
    [BeforeTestRun(Order = 1)]
    public static void RefreshDatabaseModel()
    {
        Console.WriteLine($"[{nameof(DatabasePerTestRunHook)}] {nameof(RefreshDatabaseModel)}: Refreshing database model");
        SqlDatabaseModel.Update();
    }
}