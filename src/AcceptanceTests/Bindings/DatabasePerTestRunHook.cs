using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings
{
    [Binding]
    public static class DatabasePerTestRunHook
    {
        [BeforeTestRun(Order = 1)]
        public static void RefreshDatabaseModel()
        {
            SqlDatabaseModel.Update();
        }
    }
}
