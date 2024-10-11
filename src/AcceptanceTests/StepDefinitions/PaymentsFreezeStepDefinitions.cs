using Microsoft.EntityFrameworkCore;
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

    [Given(@"the payments are frozen")]
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
        
        await WaitHelper.WaitForItAsync(async () => await ApprenticeshipFrozen(apprenticeshipKey), "Payments have not been frozen");
    }

    [Given("no payments are released for this apprenticeship")]
    [Then("no payments are released for this apprenticeship")]
    public async Task AssertCorrectPaymentsAreReleased()
    {
        var apprenticeshipKey = (Guid)_scenarioContext["apprenticeshipKey"];

        await WaitHelper.WaitForUnexpected(() =>
            FinalisedOnProgammeLearningPaymentEventHandler.ReceivedEvents.Any(x => x.ApprenticeshipKey == apprenticeshipKey),
            "No payments should have been published");

    }

    [When(@"the payments are unfrozen")]
    public async Task WhenThePaymentsAreUnfrozen()
    {
        var apprenticeshipKey = (Guid)_scenarioContext["apprenticeshipKey"];

        await _testContext.UnfreezePaymentsEndpoint.Publish(new PaymentsUnfrozenEvent
        {
            ApprenticeshipKey = apprenticeshipKey
        });

        await WaitHelper.WaitForItAsync(async () => await ApprenticeshipUnfrozen(apprenticeshipKey), "Payments have not been defrosted");
    }

    private async Task<bool> ApprenticeshipFrozen(Guid apprenticeshipKey)
    {
        var apprenticeship = await _testContext.SqlDatabase.GetApprenticeship(apprenticeshipKey);
        return apprenticeship != null && apprenticeship.PaymentsFrozen;
    }

    private async Task<bool> ApprenticeshipUnfrozen(Guid apprenticeshipKey)
    {
        var apprenticeship = await _testContext.SqlDatabase.GetApprenticeship(apprenticeshipKey);
        return apprenticeship != null && !apprenticeship.PaymentsFrozen;
    }
}
