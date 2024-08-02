using AutoFixture;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;

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

    [Given(@"the earnings started in academic year (.*) and run for (.*) years")]
    public void GivenTheEarningsStartedYear(string academicYear, int numberOfYears)
    {
        var periods = new List<DeliveryPeriod>();
        var startYear = 2000 + int.Parse(academicYear.Substring(0,2));
        var deliveryDateTime = new DateTime(startYear, 9, 1); // Starting delivery DateTime
        var duration = 12 * numberOfYears;

        for (var i = 0; i < duration; i++)
        {
            periods.Add(PeriodHelper.CreateDeliveryPeriod((byte)deliveryDateTime.Month, (short)deliveryDateTime.Year, 1000));
            deliveryDateTime = deliveryDateTime.AddMonths(1);
        }

        SetEarningsGeneratedEvent(periods, duration, 1000);
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

        SetEarningsGeneratedEvent(periods, 3, 1000);
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

        SetEarningsGeneratedEvent(periods, 3, 1000);
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
        _scenarioContext[ContextKeys.EarningsGeneratedEvent] = _earningsGeneratedEvent;
        await _testContext.EarningsGeneratedEndpoint.Publish(_earningsGeneratedEvent);
    }

    private void SetEarningsGeneratedEvent(List<DeliveryPeriod> periods, int numberOfPayments, int paymentAmount)
    {
        _earningsGeneratedEvent = _testContext.Fixture
            .Build<EarningsGeneratedEvent>()
            .With(x => x.DeliveryPeriods, periods)
            .With(x => x.Uln, _testContext.Fixture.Create<int>().ToString())
            .With(x => x.TrainingCode, _testContext.Fixture.Create<int>().ToString())
            .Create();

        _scenarioContext["numberOfPayments"] = numberOfPayments;
        _scenarioContext["paymentAmount"] = paymentAmount;
    }
}