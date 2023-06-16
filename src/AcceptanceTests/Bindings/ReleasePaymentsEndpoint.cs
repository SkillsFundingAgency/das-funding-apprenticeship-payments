using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings;

[Binding]
[Scope(Tag = "ReleasePaymentsEndpoint")]
public class ReleasePaymentsEndpoint
{
    [BeforeScenario]
    public async Task StartEndpoint(TestContext context)
    {
        if (context.ReleasePaymentsEndpoint is not null) return;

        context.ReleasePaymentsEndpoint = await EndpointHelper
            .StartEndpoint(QueueNames.ReleasePayments, true, new[] { typeof(ReleasePaymentsCommand) });
    }
}