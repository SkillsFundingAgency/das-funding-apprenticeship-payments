namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests
{
    public class TestContext : IDisposable
    {
        public TestFunction? TestFunction { get; set; }
        public AutoFixture.Fixture Fixture = new();

        public void Dispose()
        {
            Fixture = new AutoFixture.Fixture();
            TestFunction?.Dispose();
        }
    }
}
