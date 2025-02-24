using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests;

public class TestFunction : IDisposable
{
    private readonly TestServer _testServer;
    private bool _isDisposed;
    private readonly IEnumerable<QueueTriggeredFunction> _queueTriggeredFunctions;
    public string HubName { get; }


    public TestFunction(TestContext testContext, string hubName)
    {
        HubName = hubName;
        var _ = new Startup();// This forces the AzureFunction assembly to load
        _queueTriggeredFunctions = QueueFunctionResolver.GetQueueTriggeredFunctions();

        _testServer = new TestServer(new WebHostBuilder()
            .UseEnvironment(Environments.Development)
            .UseStartup<TestFunctionStartup>((_) => new TestFunctionStartup(testContext, _queueTriggeredFunctions, testContext.EndpointInstanceHandler)));
    }

    public async Task PublishEvent<T>(T eventObject)
    {
        await _queueTriggeredFunctions.PublishEvent<T>(_testServer.Services, eventObject);
    }


    public async Task WaitUntilOrchestratorComplete(string orchestratorName)
    {
        //await Jobs.WaitFor(orchestratorName, Config.TimeToWait).ThrowIfFailed();
    }
    
    public async Task DisposeAsync()
    {
        Dispose();
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