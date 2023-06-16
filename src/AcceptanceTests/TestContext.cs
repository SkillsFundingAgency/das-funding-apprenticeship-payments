using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests
{
    public class TestContext : IDisposable
    {
        public TestFunction? TestFunction { get; set; }
        public IEndpointInstance? FinalisedOnProgammeLearningPaymentEndpoint { get; set; } = null;
        public IEndpointInstance? EarningsGeneratedEndpoint { get; set; }
        public IEndpointInstance? PaymentsGeneratedEndpoint { get; set; }
        public IEndpointInstance? ReleasePaymentsEndpoint { get; set; }
        public IEndpointInstance? CalculatedRequiredLevyAmountEndpoint { get; set; }

        public AutoFixture.Fixture Fixture = new();

        public void Dispose()
        {
            FinalisedOnProgammeLearningPaymentEndpoint?.Stop();
            EarningsGeneratedEndpoint?.Stop();
            PaymentsGeneratedEndpoint?.Stop();
            ReleasePaymentsEndpoint?.Stop();
            CalculatedRequiredLevyAmountEndpoint?.Stop();
            TestFunction?.Dispose();
        }
    }
}
