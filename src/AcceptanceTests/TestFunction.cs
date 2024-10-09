using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Testing.AzureStorageEmulator;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests;

public class TestFunction : IDisposable
{
    private readonly IHost _host;
    private bool _isDisposed;

    private IJobHost Jobs => _host.Services.GetService<IJobHost>()!;
    public string HubName { get; }
    private readonly OrchestrationData _orchestrationData;

    public TestFunction(TestContext testContext, string hubName)
    {
        AzureStorageEmulatorManager.StartStorageEmulator();
        HubName = hubName;

        _orchestrationData = new OrchestrationData();

        EndpointHelper.ClearEventStorage();

        var appConfig = new Dictionary<string, string>{
            { "EnvironmentName", "LOCAL_ACCEPTANCE_TESTS" },
            { "AzureWebJobsStorage", "UseDevelopmentStorage=true" },
            { "ApplicationSettings:NServiceBusConnectionString", "UseLearningEndpoint=true;NServiceBusConnectionString" },
            { "ApplicationSettings:DCServiceBusConnectionString", "UseLearningEndpoint=true;DCServiceBusConnectionString" },
            { "ApplicationSettings:LogLevel", "DEBUG" },
            { "ApplicationSettings:DbConnectionString", testContext.SqlDatabase?.DatabaseInfo.ConnectionString! }
        };

        _host = new HostBuilder()
            .ConfigureAppConfiguration(a =>
            {
                a.Sources.Clear();
                a.AddInMemoryCollection(appConfig);
            })
            .ConfigureWebJobs(builder => builder
                .AddDurableTask(options =>
                {
                    options.HubName = HubName;
                    options.UseAppLease = false;
                    options.UseGracefulShutdown = false;
                    options.ExtendedSessionsEnabled = false;
                    options.StorageProvider["maxQueuePollingInterval"] = new TimeSpan(0, 0, 0, 0, 500);
                    options.StorageProvider["partitionCount"] = 1;
                })
                .AddAzureStorageCoreServices()
                .ConfigureServices(s =>
                {
                    builder.Services.AddLogging(options =>
                    {
                        options.SetMinimumLevel(LogLevel.Trace);
                        options.AddConsole();
                    });
                    s.Configure<ApplicationSettings>(a =>
                    {
                        a.AzureWebJobsStorage = appConfig["AzureWebJobsStorage"];
                        a.NServiceBusConnectionString = appConfig["NServiceBusConnectionString"];
                        a.DCServiceBusConnectionString = appConfig["DCServiceBusConnectionString"];
                        a.DbConnectionString = appConfig["DbConnectionString"];
                    });
                    new Startup().Configure(builder);
                    s.AddSingleton(typeof(IOrchestrationData), _orchestrationData);
                })
            )
            .Build();
    }

    public async Task StartHost()
    {
        var timeout = new TimeSpan(0, 2, 10);
        var delayTask = Task.Delay(timeout);

        await Task.WhenAny(Task.WhenAll(_host.StartAsync()), delayTask);

        if (delayTask.IsCompleted)
        {
            throw new Exception($"Failed to start test function host within {timeout.Seconds} seconds.  Check the AzureStorageEmulator is running. ");
        }
    }
    
    public async Task DisposeAsync()
    {
        await Jobs.Purge();
        await Jobs.StopAsync();
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
            _host.Dispose();
        }

        _isDisposed = true;
    }
}