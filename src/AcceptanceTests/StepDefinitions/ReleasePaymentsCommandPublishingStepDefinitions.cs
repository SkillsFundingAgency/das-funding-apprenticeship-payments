using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
public class ReleasePaymentsCommandPublishingStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private static IEndpointInstance? _endpointInstance;
    private ReleasePaymentsCommand _releasePaymentsCommand;

    public ReleasePaymentsCommandPublishingStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeTestRun]
    public static async Task StartEndpoint()
    {
        _endpointInstance = await EndpointHelper
            .StartEndpoint(QueueNames.ReleasePayments, true, new[] { typeof(ReleasePaymentsCommand) });
    }

    [AfterTestRun]
    public static async Task StopEndpoint()
    {
        await _endpointInstance.Stop()
            .ConfigureAwait(false);
    }

    [When("payments are released")]
    public async Task PublishReleasePaymentsCommand()
    {
        _releasePaymentsCommand = new ReleasePaymentsCommand
        {
            CollectionMonth = 2
        };
        await _endpointInstance.Publish(_releasePaymentsCommand);
    }
}