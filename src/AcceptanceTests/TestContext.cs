using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests
{
    public class TestContext : IDisposable
    {
        public TestFunction? TestFunction { get; set; }
        public SqlDatabase? SqlDatabase { get; set; }

        public void Dispose()
        {
            TestFunction?.Dispose();
            SqlDatabase?.Dispose();
        }
    }
}
