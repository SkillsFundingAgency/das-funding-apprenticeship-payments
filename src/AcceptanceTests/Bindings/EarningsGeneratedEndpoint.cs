using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings;

[Binding]
[Scope(Tag = "EarningsGeneratedEndpoint")]
public class EarningsGeneratedEndpoint
{
    [BeforeScenario]
    public async Task StartEndpoint(TestContext context)
    {
        if (context.EarningsGeneratedEndpoint is not null) return;

        context.EarningsGeneratedEndpoint = await EndpointHelper
            .StartEndpoint(QueueNames.EarningsGenerated, true, new[] { typeof(EarningsGeneratedEvent) });

    }
}