using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
[Scope(Feature = "Payments Release")]
public class FinalisedOnProgammeLearningPaymentEventHandlingStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;

    public FinalisedOnProgammeLearningPaymentEventHandlingStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Then("the correct payments are released")]
    public async Task AssertCorrectPaymentsAreReleased()
    {
        await WaitHelper.WaitForIt(() => FinalisedOnProgammeLearningPaymentEventHandler.ReceivedEvents.Any(ReleasedPaymentMatchesExpectation), "Failed to find published FinalisedOnProgammeLearningPaymentEvent");
    }

    private bool ReleasedPaymentMatchesExpectation(FinalisedOnProgammeLearningPaymentEvent finalisedOnProgammeLearningPaymentEvent)
    {
        return finalisedOnProgammeLearningPaymentEvent.ApprenticeshipKey == (Guid)_scenarioContext["apprenticeshipKey"] &&
               finalisedOnProgammeLearningPaymentEvent.CollectionPeriod == ((byte)DateTime.Now.Month).ToDeliveryPeriod();
    }
}