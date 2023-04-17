using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using QueueNames = SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.QueueNames;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
public class EarningsGeneratedEventPublishingStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private readonly TestContext _testContext;
    private static IEndpointInstance? _endpointInstance;
    private EarningsGeneratedEvent _earningsGeneratedEvent;

    public EarningsGeneratedEventPublishingStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
    {
        _scenarioContext = scenarioContext;
        _testContext = testContext;
    }

    [BeforeTestRun]
    public static async Task StartEndpoint()
    {
        _endpointInstance = await EndpointHelper
            .StartEndpoint(QueueNames.EarningsGenerated, true, new[] { typeof(EarningsGeneratedEvent) });
    }

    [AfterTestRun]
    public static async Task StopEndpoint()
    {
        await _endpointInstance.Stop()
            .ConfigureAwait(false);
    }

    [Given(@"earnings have been generated")]
    public void GivenEarningsHaveBeenGenerated()
    {
    }

    [Given(@"all of the earnings are due in the future")]
    public void GivenAllEarningsAreDueInTheFuture()
    {
    }

    [Given(@"no payments have previously been generated")]
    public void GivenNoPaymentsHavePreviouslyBeenGenerated()
    {
    }

    [When (@"payments are calculated")]
    public async Task PublishApprenticeshipCreatedEvent()
    {
        _earningsGeneratedEvent = new EarningsGeneratedEvent
        {
            ApprenticeshipKey = Guid.NewGuid(),
            FundingPeriods = new List<FundingPeriod>
            {
                new FundingPeriod
                {
                    DeliveryPeriods = new List<DeliveryPeriod>
                    {
                        new DeliveryPeriod { AcademicYear = 2324, Period = 1, CalenderYear = 2023, CalendarMonth = 8, LearningAmount = 1000 },
                        new DeliveryPeriod { AcademicYear = 2324, Period = 2, CalenderYear = 2023, CalendarMonth = 9, LearningAmount = 1000 }
                    }
                },
                new FundingPeriod
                {
                    DeliveryPeriods = new List<DeliveryPeriod>
                    {
                        new DeliveryPeriod { AcademicYear = 2324, Period = 3, CalenderYear = 2023, CalendarMonth = 10, LearningAmount = 1000 }
                    }
                }
            }
		};
        _scenarioContext["apprenticeshipKey"] = _earningsGeneratedEvent.ApprenticeshipKey;
        _scenarioContext["numberOfPayments"] = 3;
        _scenarioContext["paymentAmount"] = 1000;
        await _endpointInstance.Publish(_earningsGeneratedEvent);
    }
}