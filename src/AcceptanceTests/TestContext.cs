using NServiceBus;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests
{
    public class TestContext : IDisposable
    {
        public TestFunction? TestFunction { get; set; }
        public SqlDatabase? SqlDatabase { get; set; }
        public IEndpointInstance? FinalisedOnProgammeLearningPaymentEndpoint { get; set; } = null;
        public IEndpointInstance? EarningsGeneratedEndpoint { get; set; }
        public IEndpointInstance? PaymentsGeneratedEndpoint { get; set; }
        public IEndpointInstance? ReleasePaymentsEndpoint { get; set; }
        public IEndpointInstance? CalculatedRequiredLevyAmountEndpoint { get; set; }
        public IEndpointInstance? FinalisedOnProgammeLearningPaymentSendOnlyEndpoint { get; set; }
        public IEndpointInstance? EarningsRecalculatedEndpoint { get; set; }
        public IEndpointInstance? FreezePaymentsEndpoint { get; set; }
        public IEndpointInstance? UnfreezePaymentsEndpoint { get; set; }

        public AutoFixture.Fixture Fixture = new();

        public void Dispose()
        {
            FinalisedOnProgammeLearningPaymentEndpoint?.Stop();
            FinalisedOnProgammeLearningPaymentSendOnlyEndpoint?.Stop();
            EarningsGeneratedEndpoint?.Stop();
            PaymentsGeneratedEndpoint?.Stop();
            ReleasePaymentsEndpoint?.Stop();
            CalculatedRequiredLevyAmountEndpoint?.Stop();
            EarningsRecalculatedEndpoint?.Stop();
            FreezePaymentsEndpoint?.Stop();
            UnfreezePaymentsEndpoint?.Stop();
            TestFunction?.Dispose();
            SqlDatabase?.Dispose();
        }
    }
}
