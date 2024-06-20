using SFA.DAS.Apprenticeships.Types;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;
using NUnit.Framework;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
[Scope(Feature = "Payments Release")]
public class PaymentsFreezeStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private readonly TestContext _testContext;

    public PaymentsFreezeStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
    {
        _scenarioContext = scenarioContext;
        _testContext = testContext;
    }

    [When(@"the payments are frozen")]
    public async Task WhenThePaymentsAreFrozen()
    {
        var apprenticeshipKey = (Guid)_scenarioContext["apprenticeshipKey"];

        await WaitHelper.WaitForIt(() =>
            PaymentsGeneratedEventHandler.ReceivedEvents.Any(x => x.ApprenticeshipKey == apprenticeshipKey),
            "Payments need to have been generated at least once for a freeze to take place");

        await _testContext.FreezePaymentsEndpoint.Publish(new PaymentsFrozenEvent
        {
            ApprenticeshipKey = apprenticeshipKey
        });
    }

    [Then("no payments are released")]
    public async Task AssertCorrectPaymentsAreReleased()
    {
        var apprenticeshipKey = (Guid)_scenarioContext["apprenticeshipKey"];

        await WaitHelper.WaitForUnexpected(() =>
            FinalisedOnProgammeLearningPaymentEventHandler.ReceivedEvents.Any(x => x.ApprenticeshipKey == apprenticeshipKey),
            "No payments should have been published");

    }
}
