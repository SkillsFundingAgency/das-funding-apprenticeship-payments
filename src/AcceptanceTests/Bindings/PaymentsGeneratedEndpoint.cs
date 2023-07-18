using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings;

[Binding]
[Scope(Tag = "PaymentsGeneratedEndpoint")]
public class PaymentsGeneratedEndpoint
{
    [BeforeScenario]
    public static async Task StartEndpoint(TestContext context)
    {
        if (context.PaymentsGeneratedEndpoint is not null) return;

        context.PaymentsGeneratedEndpoint = await EndpointHelper
            .StartEndpoint("Test.Funding.ApprenticeshipPayments", false, new[] { typeof(PaymentsGeneratedEvent) });
    }
}