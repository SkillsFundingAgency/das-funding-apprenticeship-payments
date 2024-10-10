using AutoFixture;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
[Scope(Feature = "Calculate payments for earnings")]
[Scope(Feature = "Payments Release")]
public class EarningsGeneratedEventPublishingStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private readonly TestContext _testContext;
    private EarningsGeneratedEvent _earningsGeneratedEvent = null!;

    public EarningsGeneratedEventPublishingStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
    {
        _scenarioContext = scenarioContext;
        _testContext = testContext;
    }


    [Given(@"earnings have been generated")]
    public static void GivenEarningsHaveBeenGenerated()
    {
        // intentionally left blank
    }

    [Given(@"all of the earnings are due in the future")]
    public void GivenAllEarningsAreDueInTheFuture()
    {
        var periods = new List<DeliveryPeriod>
        {
            PeriodHelper.CreateDeliveryPeriod((byte)DateTime.Now.Month, (short)DateTime.Now.Year, 1000),
            PeriodHelper.CreateDeliveryPeriod((byte)DateTime.Now.AddMonths(1).Month, (short)DateTime.Now.AddMonths(1).Year, 1000),
            PeriodHelper.CreateDeliveryPeriod((byte)DateTime.Now.AddMonths(2).Month, (short)DateTime.Now.AddMonths(2).Year, 1000)
        };

        _earningsGeneratedEvent = _testContext.Fixture
            .Build<EarningsGeneratedEvent>()
            .With(x => x.DeliveryPeriods, periods)
            .With(x => x.Uln, _testContext.Fixture.Create<int>().ToString())
            .With(x => x.TrainingCode, _testContext.Fixture.Create<int>().ToString())
            .Create();
    }

    [Given(@"two of the earnings are due in a past month")]
    public void GivenSomeEarningsAreDueInThePast()
    {
        var periods = new List<DeliveryPeriod>
        {
             PeriodHelper.CreateDeliveryPeriod((byte)DateTime.Now.Month, (short)DateTime.Now.Year, 1000),
             PeriodHelper.CreateDeliveryPeriod((byte)DateTime.Now.AddMonths(-2).Month, (short)DateTime.Now.AddMonths(-2).Year, 1000),
             PeriodHelper.CreateDeliveryPeriod((byte)DateTime.Now.AddMonths(-1).Month, (short)DateTime.Now.AddMonths(-1).Year, 1000)
        };

        _earningsGeneratedEvent = _testContext.Fixture
            .Build<EarningsGeneratedEvent>()
            .With(x => x.DeliveryPeriods, periods)
            .With(x => x.Uln, _testContext.Fixture.Create<int>().ToString())
            .With(x => x.TrainingCode, _testContext.Fixture.Create<int>().ToString())
            .Create();
    }

    [Given(@"no payments have previously been generated")]
    public void GivenNoPaymentsHavePreviouslyBeenGenerated()
    {
        // intentionally left blank
    }

    [Given(@"payments are calculated")]
    [When (@"payments are calculated")]
    public async Task PublishApprenticeshipCreatedEvent()
    {
        _scenarioContext["apprenticeshipKey"] = _earningsGeneratedEvent.ApprenticeshipKey;
        _scenarioContext["numberOfPayments"] = 3;
        _scenarioContext["paymentAmount"] = 1000;
        _scenarioContext[ContextKeys.EarningsGeneratedEvent] = _earningsGeneratedEvent;
        await _testContext.EarningsGeneratedEndpoint.Publish(_earningsGeneratedEvent);
    }
}