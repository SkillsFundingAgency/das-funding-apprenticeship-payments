using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings;

[Binding]
[Scope(Tag = "EarningsGeneratedEndpoint")]
public static class EarningsGeneratedEndpoint
{
    [BeforeScenario]
    public static async Task StartEndpoint(TestContext context)
    {
        if (context.EarningsGeneratedEndpoint is not null) return;

        context.EarningsGeneratedEndpoint = await EndpointHelper
            .StartEndpoint(QueueNames.EarningsGenerated + "-test", true, new[] { typeof(EarningsGeneratedEvent) });

    }
}