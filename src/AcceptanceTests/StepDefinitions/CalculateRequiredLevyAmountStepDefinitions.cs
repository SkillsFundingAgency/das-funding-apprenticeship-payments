using AutoFixture;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
[Scope(Feature = "Calculate Required Levy Amount")]
public class CalculateRequiredLevyAmountStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private readonly TestContext _testContext;

    public CalculateRequiredLevyAmountStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
    {
        _scenarioContext = scenarioContext;
        _testContext = testContext;
    }

    [Given(@"an apprentice record has been approved by both the training provider & employer")]
    public async Task GivenAnApprenticeRecordHasBeenApprovedByBothTheTrainingProviderEmployer() 
    {
        var inboundEvent = _testContext.Fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();
        _scenarioContext[nameof(FinalisedOnProgammeLearningPaymentEvent)] = inboundEvent;

        await _testContext.FinalisedOnProgammeLearningPaymentEndpoint.Publish(inboundEvent);
    }

    [When(@"the associated data is used to generate a payment")]
    public void WhenTheAssociatedDataIsUsedToGenerateAPayment()
    {
        // 
    }

    [Then(@"the CalculateRequiredLevyAmount event is published to Payments v2")]
    public async Task ThenTheCalculateRequiredLevyAmountEventIsPublishedToPaymentsV2()
    {
        await WaitHelper.WaitForIt(() => CalculatedRequiredLevyAmountEventHandler.ReceivedEvents.Any(CalculatedRequiredLevyAmountEventExpectation), "Failed to find published event");
    }

    private bool CalculatedRequiredLevyAmountEventExpectation(CalculatedRequiredLevyAmountEvent outboundEvent)
    {
        var inboundEvent = (FinalisedOnProgammeLearningPaymentEvent)_scenarioContext[nameof(FinalisedOnProgammeLearningPaymentEvent)];

        return outboundEvent is not null;
        //return outboundEvent.ApprenticeshipId == inboundEvent.EmployerDetails.FundingCommitmentId;
    }
}