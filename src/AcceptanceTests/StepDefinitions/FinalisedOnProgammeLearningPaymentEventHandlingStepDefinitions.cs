using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
public class FinalisedOnProgammeLearningPaymentEventHandlingStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private static IEndpointInstance _endpointInstance;

    public FinalisedOnProgammeLearningPaymentEventHandlingStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeTestRun]
    public static async Task StartEndpoint()
    {
        _endpointInstance = await EndpointHelper
            .StartEndpoint("Test.Funding.ApprenticeshipPayments", false, new[] { typeof(FinalisedOnProgammeLearningPaymentEvent) });
    }

    [AfterTestRun]
    public static async Task StopEndpoint()
    {
        await _endpointInstance.Stop()
            .ConfigureAwait(false);
    }

    [Then("the correct payments are released")]
    public async Task AssertCorrectPaymentsAreReleased()
    {
        await WaitHelper.WaitForIt(() => FinalisedOnProgammeLearningPaymentEventHandler.ReceivedEvents.Any(ReleasedPaymentMatchesExpectation), "Failed to find published FinalisedOnProgammeLearningPaymentEvent");
    }

    private bool ReleasedPaymentMatchesExpectation(FinalisedOnProgammeLearningPaymentEvent finalisedOnProgammeLearningPaymentEvent)
    {
        return finalisedOnProgammeLearningPaymentEvent.ApprenticeshipKey == (Guid)_scenarioContext["apprenticeshipKey"] &&
               finalisedOnProgammeLearningPaymentEvent.CollectionMonth == ((byte)DateTime.Now.Month).ToDeliveryPeriod();
    }
}