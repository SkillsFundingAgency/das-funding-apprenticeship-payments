using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings;

[Binding]
[Scope(Tag = "FinalisedOnProgammeLearningPaymentEndpoint")]
public class FinalisedOnProgammeLearningPaymentEndpoint
{
    [BeforeScenario]
    public async Task StartEndpoint(TestContext context)
    {
        if (context.FinalisedOnProgammeLearningPaymentEndpoint is not null) return;

        context.FinalisedOnProgammeLearningPaymentEndpoint = await EndpointHelper
            .StartEndpoint(QueueNames.FinalisedOnProgammeLearningPayment, false, new[] { typeof(FinalisedOnProgammeLearningPaymentEvent) });

    }
}