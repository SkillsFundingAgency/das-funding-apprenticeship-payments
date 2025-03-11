using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Hosting;
using NServiceBus.Testing;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Orchestration;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests;

public class TestFunction : IDisposable
{
    private readonly TestServer _testServer;
    private bool _isDisposed;
    private readonly IEnumerable<MessageHandler> _queueTriggeredFunctions;
    public string HubName { get; }


    public TestFunction(TestContext testContext, string hubName)
    {
        HubName = hubName;
        var _ = new Startup();// This forces the AzureFunction assembly to load
        _queueTriggeredFunctions = MessageHandlerHelper.GetMessageHandlers();

        _testServer = new TestServer(new WebHostBuilder()
            .UseEnvironment(Environments.Development)
            .UseStartup<TestFunctionStartup>((_)=> new TestFunctionStartup(testContext, _queueTriggeredFunctions, testContext.EndpointInstanceHandler)));

    }

    public async Task PublishEvent<T>(T eventObject)
    {
        var function = _queueTriggeredFunctions.FirstOrDefault(x => x.HandledEventType == typeof(T));
        var handler = _testServer.Services.GetService(function.HandlerType) as IHandleMessages<T>;
        var context = new TestableMessageHandlerContext
        {
            CancellationToken = new CancellationToken()
        };
        await handler.Handle(eventObject, context);
    }


    public async Task WaitUntilOrchestratorComplete(string orchestratorName)
    {
        var orchestrator = (DurableTaskClient)_testServer.Services.GetService(typeof(DurableTaskClient))!;
        var orchestrations = orchestrator!.GetAllInstancesAsync().ToList<OrchestrationMetadata>();
        var instanceId = orchestrations.First(x => x.Name == orchestratorName).InstanceId;
        await orchestrator.WaitForInstanceCompletionAsync(instanceId);
    }

    public Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            // no components to dispose
        }

        _isDisposed = true;
    }
}