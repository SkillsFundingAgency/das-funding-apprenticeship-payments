using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings;

[Binding]
[Scope(Tag = "FinalisedOnProgammeLearningPaymentSendOnlyEndpoint")]
public static class FinalisedOnProgammeLearningPaymentSendOnlyEndpoint
{
    [BeforeScenario]
    public static async Task StartEndpoint(TestContext context)
    {
        if (context.FinalisedOnProgammeLearningPaymentSendOnlyEndpoint is not null) return;

        context.FinalisedOnProgammeLearningPaymentSendOnlyEndpoint = await EndpointHelper
            .StartEndpoint(QueueNames.FinalisedOnProgammeLearningPayment + "-test", true, new[] { typeof(FinalisedOnProgammeLearningPaymentEvent) });

    }
}