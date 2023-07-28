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
        await WaitHelper.WaitForIt(() => FinalisedOnProgammeLearningPaymentEventHandler.ReceivedEvents.Any(ReleasedPaymentMatchesExpectation), "Failed to find published FinalisedOnProgammeLearningPaymentEvent");
    }

    private bool ReleasedPaymentMatchesExpectation(FinalisedOnProgammeLearningPaymentEvent finalisedOnProgammeLearningPaymentEvent)
    {
        var earningsGeneratedEvent = (EarningsGeneratedEvent)_scenarioContext[ContextKeys.EarningsGeneratedEvent];

        if (finalisedOnProgammeLearningPaymentEvent.ApprenticeshipKey != (Guid)_scenarioContext["apprenticeshipKey"]) return false;

        finalisedOnProgammeLearningPaymentEvent.ApprenticeshipKey.Should().Be(earningsGeneratedEvent.ApprenticeshipKey);
        finalisedOnProgammeLearningPaymentEvent.CollectionPeriod.Should().Be(((byte)DateTime.Now.Month).ToDeliveryPeriod());
        finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.Uln.ToString().Should().Be(earningsGeneratedEvent.Uln);
        finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.StartDate.Should().Be(earningsGeneratedEvent.StartDate);
        finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.PlannedEndDate.Should().Be(earningsGeneratedEvent.PlannedEndDate);
        finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.ProviderIdentifier.Should().Be(earningsGeneratedEvent.ProviderId);

        var expectedAmount = earningsGeneratedEvent.DeliveryPeriods.First(x => x.Period == ((byte)DateTime.Now.Month).ToDeliveryPeriod()).LearningAmount; 
        finalisedOnProgammeLearningPaymentEvent.Amount.Should().Be(expectedAmount);


        return true;

    }
}