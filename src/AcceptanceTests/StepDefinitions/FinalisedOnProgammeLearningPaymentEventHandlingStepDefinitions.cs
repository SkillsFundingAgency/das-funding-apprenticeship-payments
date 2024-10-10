using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
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
        await WaitHelper.WaitForIt(() => FinalisedOnProgammeLearningPaymentEventHandler.ReceivedEvents.Any(ReleasedPaymentMatchesExpectation), $"Failed to find expected published FinalisedOnProgammeLearningPaymentEvent when asserting correct payments are released");
    }

    [Then("the correct unfrozen payments are released")]
    public async Task AssertCorrectUnfrozenPaymentsAreReleased()
    {
        await WaitHelper.WaitForIt(() => FinalisedOnProgammeLearningPaymentEventHandler.ReceivedEvents.Any(ReleasedPaymentMatchesUnfrozenExpectation), "Failed to find expected published FinalisedOnProgammeLearningPaymentEvent when asserting correct unfrozen payments are released");
    }

    private bool ReleasedPaymentMatchesExpectation(FinalisedOnProgammeLearningPaymentEvent finalisedOnProgammeLearningPaymentEvent)
    {
        var earningsGeneratedEvent = (EarningsGeneratedEvent)_scenarioContext[ContextKeys.EarningsGeneratedEvent];

        if (finalisedOnProgammeLearningPaymentEvent.ApprenticeshipKey != (Guid)_scenarioContext["apprenticeshipKey"]) return false;

        var expectedAmount = earningsGeneratedEvent.DeliveryPeriods.First(x => x.Period == ((byte)DateTime.Now.Month).ToDeliveryPeriod()).LearningAmount;

        return 
            finalisedOnProgammeLearningPaymentEvent.ApprenticeshipKey == earningsGeneratedEvent.ApprenticeshipKey
            && finalisedOnProgammeLearningPaymentEvent.CollectionPeriod == ((byte)DateTime.Now.Month).ToDeliveryPeriod()
            && finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.Uln.ToString() == earningsGeneratedEvent.Uln
            && finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.StartDate.Date == earningsGeneratedEvent.StartDate.Date
            && finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.PlannedEndDate?.Date == earningsGeneratedEvent.PlannedEndDate.Date
            && finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.ProviderIdentifier == earningsGeneratedEvent.ProviderId
            && finalisedOnProgammeLearningPaymentEvent.Amount == expectedAmount;
    }

    private bool ReleasedPaymentMatchesUnfrozenExpectation(FinalisedOnProgammeLearningPaymentEvent finalisedOnProgammeLearningPaymentEvent)
    {
        var earningsGeneratedEvent = (EarningsGeneratedEvent)_scenarioContext[ContextKeys.EarningsGeneratedEvent];

        if (finalisedOnProgammeLearningPaymentEvent.ApprenticeshipKey != (Guid)_scenarioContext["apprenticeshipKey"]) return false;

        var expectedAmount = earningsGeneratedEvent.DeliveryPeriods.First(x => x.Period == ((byte)DateTime.Now.Month).ToDeliveryPeriod()).LearningAmount;

        return
            finalisedOnProgammeLearningPaymentEvent.ApprenticeshipKey == earningsGeneratedEvent.ApprenticeshipKey
            && finalisedOnProgammeLearningPaymentEvent.CollectionPeriod == ((byte)DateTime.Now.AddMonths(1).Month).ToDeliveryPeriod()
            && finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.Uln.ToString() == earningsGeneratedEvent.Uln
            && finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.StartDate.Date == earningsGeneratedEvent.StartDate.Date
            && finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.PlannedEndDate?.Date == earningsGeneratedEvent.PlannedEndDate.Date
            && finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.ProviderIdentifier == earningsGeneratedEvent.ProviderId
            && finalisedOnProgammeLearningPaymentEvent.Amount == expectedAmount
            && finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.DeliveryPeriod == ((byte)DateTime.Now.Month).ToDeliveryPeriod();
    }
}