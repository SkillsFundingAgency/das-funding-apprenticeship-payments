using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings;

[Binding]
[Scope(Tag = "UnfreezePaymentsEndpoint")]
public static class UnfreezePaymentsEndpoint
{
    [BeforeScenario]
    public static async Task StartEndpoint(TestContext context)
    {
        if (context.UnfreezePaymentsEndpoint is not null) return;

        context.UnfreezePaymentsEndpoint = await EndpointHelper
            .StartEndpoint(QueueNames.PaymentsUnfrozen + "-test", true, new[] { typeof(PaymentsUnfrozenEvent) });

    }
}