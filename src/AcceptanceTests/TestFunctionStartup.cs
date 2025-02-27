using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Orchestration;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests;

internal class TestFunctionStartup
{
    private readonly Startup _startUp;
    private readonly IEnumerable<QueueTriggeredFunction> _queueTriggeredFunctions;
    private readonly TestEndpointInstanceHandler _testEndpointInstanceHandler;
    private readonly TestOuterApi _testOuterApi;

    public TestFunctionStartup(
        TestContext testContext,
        IEnumerable<QueueTriggeredFunction> queueTriggeredFunctions,
        TestEndpointInstanceHandler testEndpointInstanceHandler)
    {
        _startUp = new Startup();
        _startUp.Configuration = GenerateConfiguration(testContext);
        _queueTriggeredFunctions = queueTriggeredFunctions;
        _testEndpointInstanceHandler = testEndpointInstanceHandler;
        _testOuterApi = new TestOuterApi(testContext);
    }

    public void Configure()
    {
        // Intentionally left blank
    }

    public void ConfigureServices(IServiceCollection collection)
    {
        _startUp.SetupServices(collection);

        collection.AddSingleton<IDasServiceBusEndpoint>(new DasServiceBusEndpoint(_testEndpointInstanceHandler));
        collection.AddSingleton<IPaymentsV2ServiceBusEndpoint>(new PaymentsV2ServiceBusEndpoint(_testEndpointInstanceHandler));

        foreach (var queueTriggeredFunction in _queueTriggeredFunctions)
        {
            collection.AddTransient(queueTriggeredFunction.ClassType);
        }

        var orchestrationFunctions = OrchestrationFunctionResolver.GetTriggeredFunctions();
        foreach (var orchestrationTriggeredFunction in orchestrationFunctions)
        {
            collection.AddTransient(orchestrationTriggeredFunction.ClassType);
        }

        collection.AddSingleton<ISystemClockService, TestSystemClock>();// override DI in Startup, must come after new Startup().Configure(builder);
        collection.AddSingleton<IOuterApiClient>(_testOuterApi);// override DI in Startup, must come after new Startup().Configure(builder);

        collection.AddSingleton<DurableTaskClient>(sp => 
            new InMemoryDurableTaskClient("TestHub", new FunctionInvoker(sp, orchestrationFunctions))
        );
    }

    private static IConfigurationRoot GenerateConfiguration(TestContext testContext)
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new[]
            {
                new KeyValuePair<string, string?>("EnvironmentName", "LOCAL_ACCEPTANCE_TESTS"),
                new KeyValuePair<string, string?>("AzureWebJobsStorage", "UseDevelopmentStorage=true"),
                new KeyValuePair<string, string?>("AzureWebJobsServiceBus", "UseDevelopmentStorage=true"),
                new KeyValuePair<string, string?>("ApplicationSettings:NServiceBusConnectionString", "UseLearningEndpoint=true"),
                new KeyValuePair<string, string?>("ApplicationSettings:DCServiceBusConnectionString", "UseLearningEndpoint=true"),
                new KeyValuePair<string, string?>("ApplicationSettings:LogLevel", "DEBUG"),
                new KeyValuePair<string, string?>("ApplicationSettings:DbConnectionString", testContext.SqlDatabase?.DatabaseInfo.ConnectionString!),
                new KeyValuePair<string, string?>("ApprenticeshipsOuterApi:Key","" ),
                new KeyValuePair<string, string?>("ApprenticeshipsOuterApi:BaseUrl","https://localhost:7101/" ),
                new KeyValuePair<string, string?>("PaymentsOuterApi:Key","" ),
                new KeyValuePair<string, string?>("PaymentsOuterApi:BaseUrl","https://localhost:7102/" )
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);
        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}
