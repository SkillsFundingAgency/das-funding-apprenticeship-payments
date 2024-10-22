using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Orchestrators;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
[Scope(Feature = "Payments Release")]
public class ReleasePaymentsCommandPublishingStepDefinitions
{
    private readonly TestContext _testContext;
    private ReleasePaymentsCommand _releasePaymentsCommand = null!;
    private ISystemClockService _systemClockService;

    public ReleasePaymentsCommandPublishingStepDefinitions(TestContext testContext)
    {
        _testContext = testContext;
        _systemClockService = TestSystemClock.Instance();
    }

    [Given("payments are released")]
    [When("payments are released")]
    public async Task PublishReleasePaymentsCommand()
    {
        _releasePaymentsCommand = new ReleasePaymentsCommand
        {
            CollectionPeriod = ((byte)_systemClockService.Now.Month).ToDeliveryPeriod(),
            CollectionYear = ((short)_systemClockService.Now.Year).ToAcademicYear((byte)DateTime.Now.Month)
        };
        await ReleasePayments();
    }

    [Given(@"payments are released every month until (.*)")]
    [When(@"payments are released every month until (.*)")]
    public async Task PublishReleasePaymentsCommandUntil(string endDateString)
    {
        var releaseDate = _systemClockService.Now;
        var endDate = DateTime.Parse(endDateString).AddDays(1);

        while (releaseDate < endDate)
        {
            _releasePaymentsCommand = new ReleasePaymentsCommand
            {
                CollectionPeriod = ((byte)releaseDate.Month).ToDeliveryPeriod(),
                CollectionYear = ((short)releaseDate.Year).ToAcademicYear((byte)releaseDate.Month)
            };
            await ReleasePayments();
            releaseDate = releaseDate.AddMonths(1);
        }
    }

    [When("payments are released for the next collection period")]
    public async Task PublishReleasePaymentsCommandNextCollectionPeriod()
    {
        var nextCollectionPeriodDate = _systemClockService.Now.AddMonths(1);
        _releasePaymentsCommand = new ReleasePaymentsCommand
        {
            CollectionPeriod = ((byte)nextCollectionPeriodDate.Month).ToDeliveryPeriod(),
            CollectionYear = ((short)nextCollectionPeriodDate.Year).ToAcademicYear((byte)nextCollectionPeriodDate.Month)
        };
        await ReleasePayments();
    }

    private async Task ReleasePayments()
    {
        await _testContext.ReleasePaymentsEndpoint.Publish(_releasePaymentsCommand);
        await _testContext.TestFunction.WaitUntilOrchestratorComplete(nameof(ReleasePaymentsOrchestrator));
    }
}