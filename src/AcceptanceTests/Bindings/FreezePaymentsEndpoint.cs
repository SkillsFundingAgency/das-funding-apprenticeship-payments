using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings;

[Binding]
[Scope(Tag = "FreezePaymentsEndpoint")]
public static class FreezePaymentsEndpoint
{
    [BeforeScenario]
    public static async Task StartEndpoint(TestContext context)
    {
        if (context.FreezePaymentsEndpoint is not null) return;

        context.FreezePaymentsEndpoint = await EndpointHelper
            .StartEndpoint(QueueNames.PaymentsFrozen + "-test", true, new[] { typeof(PaymentsFrozenEvent) });

    }
}