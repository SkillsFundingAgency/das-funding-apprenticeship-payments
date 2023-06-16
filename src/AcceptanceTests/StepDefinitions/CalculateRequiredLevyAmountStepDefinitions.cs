using AutoFixture;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;
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
    private static IEndpointInstance? _endpointInstance;

    public CalculateRequiredLevyAmountStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
    {
        _scenarioContext = scenarioContext;
        _testContext = testContext;
    }

    [BeforeTestRun]
    public static async Task StartEndpoint()
    {
        _endpointInstance = await EndpointHelper
            .StartEndpoint(QueueNames.FinalisedOnProgammeLearningPayment, true, new[] { typeof(FinalisedOnProgammeLearningPaymentEvent) });
    }

    [AfterTestRun]
    [Scope(Feature = "Calculate Required Levy Amount")]
    public static void StopEndpoint() => _endpointInstance?.Stop();

    [Given(@"an apprentice record has been approved by both the training provider & employer")]
    public async Task GivenAnApprenticeRecordHasBeenApprovedByBothTheTrainingProviderEmployer()
    {
        var inboundEvent = _testContext.Fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();
        _scenarioContext[nameof(FinalisedOnProgammeLearningPaymentEvent)] = inboundEvent;

        await _endpointInstance.Publish(inboundEvent);
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

    private bool CalculatedRequiredLevyAmountEventExpectation(CalculatedRequiredLevyAmount outboundEvent)
    {
        var inboundEvent = (FinalisedOnProgammeLearningPaymentEvent)_scenarioContext[nameof(FinalisedOnProgammeLearningPaymentEvent)];

        return outboundEvent.ApprenticeshipId == inboundEvent.EmployerDetails.FundingCommitmentId;
    }
}