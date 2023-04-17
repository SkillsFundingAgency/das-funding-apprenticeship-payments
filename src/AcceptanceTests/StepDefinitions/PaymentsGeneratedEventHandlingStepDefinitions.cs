using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
public class PaymentsGeneratedEventHandlingStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private static IEndpointInstance _endpointInstance;

    public PaymentsGeneratedEventHandlingStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeTestRun]
    public static async Task StartEndpoint()
    {
        _endpointInstance = await EndpointHelper
            .StartEndpoint("Test.Funding.ApprenticeshipPayments", false, new[] { typeof(PaymentsGeneratedEvent) });
    }

    [AfterTestRun]
    public static async Task StopEndpoint()
    {
        await _endpointInstance.Stop()
            .ConfigureAwait(false);
    }

    [Then(@"Payments are generated with the correct learning amounts")]
    public async Task AssertEarningsGeneratedEvent()
    {
        await WaitHelper.WaitForIt(() => PaymentsGeneratedEventHandler.ReceivedEvents.Any(EventMatchesExpectation), "Failed to find published PaymentsGenerated event");
    }

    private bool EventMatchesExpectation(PaymentsGeneratedEvent paymentsGeneratedEvent)
    {
        return paymentsGeneratedEvent.ApprenticeshipKey == (Guid) _scenarioContext["apprenticeshipKey"] &&
               paymentsGeneratedEvent.Payments.Count() == (int) _scenarioContext["numberOfPayments"] &&
               paymentsGeneratedEvent.Payments.All(x => x.Amount == (int) _scenarioContext["paymentAmount"]);
    }
}