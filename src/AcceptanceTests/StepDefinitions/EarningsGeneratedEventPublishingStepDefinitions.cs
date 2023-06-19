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
    private EarningsGeneratedEvent _earningsGeneratedEvent;

    public EarningsGeneratedEventPublishingStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
    {
        _scenarioContext = scenarioContext;
        _testContext = testContext;
    }


    [Given(@"earnings have been generated")]
    public void GivenEarningsHaveBeenGenerated()
    {
    }

    [Given(@"all of the earnings are due in the future")]
    public void GivenAllEarningsAreDueInTheFuture()
    {
        _earningsGeneratedEvent = new EarningsGeneratedEvent
        {
            ApprenticeshipKey = Guid.NewGuid(),
            FundingPeriods = new List<FundingPeriod>
            {
                new()
                {
                    DeliveryPeriods = new List<DeliveryPeriod>
                    {
                        new() { CalenderYear = (short)DateTime.Now.Year, CalendarMonth = (byte)DateTime.Now.Month, LearningAmount = 1000 },
                        new() { CalenderYear = (short)DateTime.Now.AddMonths(1).Year, CalendarMonth = (byte)DateTime.Now.AddMonths(1).Month, LearningAmount = 1000 }
                    }
                },
                new()
                {
                    DeliveryPeriods = new List<DeliveryPeriod>
                    {
                        new() { CalenderYear = (short)DateTime.Now.AddMonths(2).Year, CalendarMonth = (byte)DateTime.Now.AddMonths(2).Month, LearningAmount = 1000 }
                    }
                }
            }
        };

        _earningsGeneratedEvent.SetDeliveryPeriodsAccordingToCalendarMonths();
    }

    [Given(@"two of the earnings are due in a past month")]
    public void GivenSomeEarningsAreDueInThePast()
    {
        _earningsGeneratedEvent = new EarningsGeneratedEvent
        {
            ApprenticeshipKey = Guid.NewGuid(),
            FundingPeriods = new List<FundingPeriod>
            {
                new()
                {
                    DeliveryPeriods = new List<DeliveryPeriod>
                    {
                        new() { CalenderYear = (short)DateTime.Now.AddMonths(-2).Year, CalendarMonth = (byte)DateTime.Now.AddMonths(-2).Month, LearningAmount = 1000 },
                        new() { CalenderYear = (short)DateTime.Now.AddMonths(-1).Year, CalendarMonth = (byte)DateTime.Now.AddMonths(-1).Month, LearningAmount = 1000 }
                    }
                },
                new()
                {
                    DeliveryPeriods = new List<DeliveryPeriod>
                    {
                        new() { CalenderYear = (short)DateTime.Now.Year, CalendarMonth = (byte)DateTime.Now.Month, LearningAmount = 1000 }
                    }
                }
            }
        };

        _earningsGeneratedEvent.SetDeliveryPeriodsAccordingToCalendarMonths();
    }

    [Given(@"no payments have previously been generated")]
    public void GivenNoPaymentsHavePreviouslyBeenGenerated()
    {
    }

    [When (@"payments are calculated")]
    public async Task PublishApprenticeshipCreatedEvent()
    {
        _scenarioContext["apprenticeshipKey"] = _earningsGeneratedEvent.ApprenticeshipKey;
        _scenarioContext["numberOfPayments"] = 3;
        _scenarioContext["paymentAmount"] = 1000;
        await _testContext.EarningsGeneratedEndpoint.Publish(_earningsGeneratedEvent);
    }
}