using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings;

[Binding]
[Scope(Tag = "ReleasePaymentsEndpoint")]
public static class ReleasePaymentsEndpoint
{
    [BeforeScenario]
    public static async Task StartEndpoint(TestContext context)
    {
        if (context.ReleasePaymentsEndpoint is not null) return;

        context.ReleasePaymentsEndpoint = await EndpointHelper
            .StartEndpoint(QueueNames.ReleasePayments + "-test", true, new[] { typeof(ReleasePaymentsCommand) });
    }
}