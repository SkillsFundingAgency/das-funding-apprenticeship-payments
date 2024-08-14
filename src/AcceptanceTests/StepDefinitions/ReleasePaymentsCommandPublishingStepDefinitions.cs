using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
[Scope(Feature = "Payments Release")]
public class ReleasePaymentsCommandPublishingStepDefinitions
{
    private readonly TestContext _testContext;
    private ReleasePaymentsCommand _releasePaymentsCommand = null!;

    public ReleasePaymentsCommandPublishingStepDefinitions(TestContext testContext)
    {
        _testContext = testContext;
    }

    [Given("payments are released")]
    [When("payments are released")]
    public async Task PublishReleasePaymentsCommand()
    {
        _releasePaymentsCommand = new ReleasePaymentsCommand
        {
            CollectionPeriod = ((byte)DateTime.Now.Month).ToDeliveryPeriod(),
            CollectionYear = ((short)DateTime.Now.Year).ToAcademicYear((byte)DateTime.Now.Month)
        };
        await _testContext.ReleasePaymentsEndpoint.Publish(_releasePaymentsCommand);
    }

    [When("payments are released for the next collection period")]
    public async Task PublishReleasePaymentsCommandNextCollectionPeriod()
    {
        var nextCollectionPeriodDate = DateTime.Now.AddMonths(1);
        _releasePaymentsCommand = new ReleasePaymentsCommand
        {
            CollectionPeriod = ((byte)nextCollectionPeriodDate.Month).ToDeliveryPeriod(),
            CollectionYear = ((short)nextCollectionPeriodDate.Year).ToAcademicYear((byte)nextCollectionPeriodDate.Month)
        };
        await _testContext.ReleasePaymentsEndpoint.Publish(_releasePaymentsCommand);
    }

}