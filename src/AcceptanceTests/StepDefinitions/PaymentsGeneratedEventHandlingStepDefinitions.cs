using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
[Scope(Feature = "Calculate payments for earnings")]
[Scope(Feature = "Payments Release")]
public class PaymentsGeneratedEventHandlingStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private readonly TestContext _testContext;

    public PaymentsGeneratedEventHandlingStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
    {
        _scenarioContext = scenarioContext;
        _testContext = testContext;
    }

    [Given(@"payments are generated with the correct learning amounts")]
    [Then(@"payments are generated with the correct learning amounts")]
    [When(@"payments are generated with the correct learning amounts")]
    public async Task AssertFutureEarningsGeneratedEvent()
    {
        await WaitHelper.WaitForIt(() => _testContext.ReceivedEvents<PaymentsGeneratedEvent>().Any(FuturePaymentsMatchExpectation), "Failed to find published PaymentsGenerated event");
    }

    [Then(@"the past earnings are allocated to the current month")]
    public async Task AssertPastEarningsGeneratedEvent()
    {
        await WaitHelper.WaitForIt(() => _testContext.ReceivedEvents<PaymentsGeneratedEvent>().Any(PastPaymentsMatchExpectation), "Failed to find published PaymentsGenerated event");
    }

    private bool FuturePaymentsMatchExpectation(PaymentsGeneratedEvent paymentsGeneratedEvent)
    {
        return paymentsGeneratedEvent.ApprenticeshipKey == (Guid)_scenarioContext["apprenticeshipKey"] &&
               paymentsGeneratedEvent.Payments.Count == (int)_scenarioContext["numberOfPayments"] &&
               paymentsGeneratedEvent.Payments.TrueForAll(x => x.Amount == (int)_scenarioContext["paymentAmount"]);
    }

    private bool PastPaymentsMatchExpectation(PaymentsGeneratedEvent paymentsGeneratedEvent)
    {
        return paymentsGeneratedEvent.ApprenticeshipKey == (Guid)_scenarioContext["apprenticeshipKey"] &&
               paymentsGeneratedEvent.Payments.Count == (int)_scenarioContext["numberOfPayments"] &&
               paymentsGeneratedEvent.Payments.TrueForAll(x => x.Amount == (int)_scenarioContext["paymentAmount"]);
    }
}