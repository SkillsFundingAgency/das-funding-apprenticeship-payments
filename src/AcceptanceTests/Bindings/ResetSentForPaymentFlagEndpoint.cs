using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings;

[Binding]
[Scope(Tag = "ResetSentForPaymentFlagEndpoint")]
public static class ResetSentForPaymentFlagEndpoint
{
    [BeforeScenario]
    public static async Task StartEndpoint(TestContext context)
    {
        if (context.ResetSentForPaymentFlagEndpoint is not null) return;

        context.ResetSentForPaymentFlagEndpoint = await EndpointHelper
            .StartEndpoint(QueueNames.ResetSentForPaymentFlag + "-test", true, new[] { typeof(ResetSentForPaymentFlagForCollectionPeriodDurableEntityCommand) });
    }
}