using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
[Scope(Feature = "Payments Release")]
public class ReleasePaymentsCommandPublishingStepDefinitions
{
    private readonly TestContext _testContext;
    private ReleasePaymentsCommand _releasePaymentsCommand;

    public ReleasePaymentsCommandPublishingStepDefinitions(TestContext testContext)
    {
        _testContext = testContext;
    }

    [When("payments are released")]
    public async Task PublishReleasePaymentsCommand()
    {
        _releasePaymentsCommand = new ReleasePaymentsCommand
        {
            CollectionPeriod = ((byte)DateTime.Now.Month).ToDeliveryPeriod()
        };
        await _testContext.ReleasePaymentsEndpoint.Publish(_releasePaymentsCommand);
    }
}