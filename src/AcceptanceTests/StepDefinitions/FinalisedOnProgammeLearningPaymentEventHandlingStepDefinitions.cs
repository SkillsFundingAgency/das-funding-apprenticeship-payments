using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
[Scope(Feature = "Payments Release")]
public class FinalisedOnProgammeLearningPaymentEventHandlingStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private readonly TestContext _testContext;

    public FinalisedOnProgammeLearningPaymentEventHandlingStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
    {
        _scenarioContext = scenarioContext;
        _testContext = testContext;
    }

    [When("the correct payments are released")]
    [Then("the correct payments are released")]
    public async Task AssertCorrectPaymentsAreReleased()
    {
        await WaitHelper.WaitForIt(() => _testContext.ReceivedEvents<FinalisedOnProgammeLearningPaymentEvent>().Any(ReleasedPaymentMatchesExpectation), $"Failed to find expected published FinalisedOnProgammeLearningPaymentEvent when asserting correct payments are released");
    }

    [Then("the correct payments are re-released")]
    public async Task AssertCorrectPaymentsAreReReleased()
    {
        await WaitHelper.WaitForIt(() => _testContext.ReceivedEvents<FinalisedOnProgammeLearningPaymentEvent>().Count(ReleasedPaymentMatchesExpectation) == 2, $"Failed to find expected published FinalisedOnProgammeLearningPaymentEvent(s) when asserting correct payments are re-released following a reset of the SentForPaymentsFlag");
    }

    [Then("multiple copies of the same payment are not released")]
    public async Task AssertDuplicatedPaymentsNotReleased()
    {
        await WaitHelper.WaitForUnexpected(() =>
        {
            return _testContext.ReceivedEvents<FinalisedOnProgammeLearningPaymentEvent>().Count(x =>
                x.ApprenticeshipKey == (Guid)_scenarioContext["apprenticeshipKey"] && x.CollectionPeriod == ((byte)DateTime.Now.Month).ToDeliveryPeriod()) > 1;
        }, $"Found unexpected event for apprenticeship");
    }

    [Then("the correct unfrozen payments are released")]
    public async Task AssertCorrectUnfrozenPaymentsAreReleased()
    {
        await WaitHelper.WaitForIt(() => _testContext.ReceivedEvents<FinalisedOnProgammeLearningPaymentEvent>().Any(ReleasedPaymentMatchesUnfrozenExpectation), "Failed to find expected published FinalisedOnProgammeLearningPaymentEvent when asserting correct unfrozen payments are released");
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