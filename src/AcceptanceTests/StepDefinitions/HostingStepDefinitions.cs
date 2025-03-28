using System.Diagnostics;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
public class HostingStepDefinitions
{
    private readonly TestContext _testContext;
    private readonly FeatureContext _featureContext;

    public HostingStepDefinitions(TestContext testContext, FeatureContext featureContext)
    {
        _testContext = testContext;
        _featureContext = featureContext;
    }

    [BeforeScenario(Order = 3)]
    public async Task CreateConfig()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        _testContext.TestFunction = new TestFunction(_testContext, $"TEST{_featureContext.FeatureInfo.Title.Replace(" ", "")}");

        TestSystemClock.SetDateTime(DateTime.Now); //Default to current time, can be overridden in tests where needed
        stopwatch.Stop();
        Console.WriteLine($"Time it took to spin up Azure Functions Host: {stopwatch.Elapsed.Milliseconds} milliseconds for hub {_testContext.TestFunction.HubName}");
    }

    [AfterScenario(Order = 101)]
    public async Task CleanupAfterTestHarness()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        await _testContext.TestFunction?.DisposeAsync()!;
        stopwatch.Stop();
        Console.WriteLine($"Time it took to Cleanup  FunctionsHost: {stopwatch.Elapsed.Milliseconds} milliseconds for hub {_testContext.TestFunction.HubName}");
    }
}