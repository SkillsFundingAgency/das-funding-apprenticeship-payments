using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings;

[Binding]
[Scope(Tag = "EarningsRecalculatedEndpoint")]
public static class EarningsRecalculatedEndpoint
{
    [BeforeScenario]
    public static async Task StartEndpoint(TestContext context)
    {
        if (context.EarningsRecalculatedEndpoint is not null) return;

        context.EarningsRecalculatedEndpoint = await EndpointHelper
            .StartEndpoint(QueueNames.EarningsRecalculated + "-test", true, new[] { typeof(ApprenticeshipEarningsRecalculatedEvent) });

    }
}