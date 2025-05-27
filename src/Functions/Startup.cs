using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Options;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.AppStart;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Extensions;
using SFA.DAS.Funding.ApprenticeshipPayments.Query;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions;

[ExcludeFromCodeCoverage]
public class Startup
{
    public IConfiguration Configuration { get; set; }

    private ApplicationSettings _applicationSettings;

    public ApplicationSettings ApplicationSettings
    {
        get
        {
            if (_applicationSettings == null)
            {
                _applicationSettings = new ApplicationSettings();
                Configuration.Bind(nameof(ApplicationSettings), _applicationSettings);
            }
            return _applicationSettings;
        }
    }

    public Startup()
    {
    }

    public void Configure(IHostBuilder builder)
    {
         builder
        .ConfigureFunctionsWorkerDefaults(options => options.UseFunctionExecutionMiddleware())
        .ConfigureAppConfiguration(PopulateConfig)
        .ConfigureNServiceBusForSubscribe()
        .ConfigureServices((c, s) =>
        {
            SetupServices(s);
            s.ConfigureNServiceBusForSend<IDasServiceBusEndpoint>(
                ApplicationSettings.NServiceBusConnectionString.GetFullyQualifiedNamespace(), 
                (endpointInstance) => new DasServiceBusEndpoint(endpointInstance));

            s.ConfigureNServiceBusForSend<IPaymentsV2ServiceBusEndpoint>(
                ApplicationSettings.DCServiceBusConnectionString.GetFullyQualifiedNamespace(),
                (endpointInstance) => new PaymentsV2ServiceBusEndpoint(endpointInstance));

            s.AddFunctionHealthChecks(ApplicationSettings, Configuration.NotLocalOrAcceptanceTests());
        });
    }

    private void PopulateConfig(IConfigurationBuilder configurationBuilder)
    {
        Environment.SetEnvironmentVariable("ENDPOINT_NAME", "SFA.DAS.Funding.ApprenticeshipPayments");

        configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", true);

        var configuration = configurationBuilder.Build();

        configurationBuilder.AddAzureTableStorage(options =>
        {
            options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
            options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
            options.EnvironmentName = configuration["EnvironmentName"];
            options.PreFixConfigurationKeys = false;
        });

        Configuration = configurationBuilder.Build();
    }

    public void SetupServices(IServiceCollection services)
    {
        services.Configure<PaymentsOuterApi>(Configuration.GetSection(nameof(PaymentsOuterApi)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<PaymentsOuterApi>>()!.Value);

        services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), Configuration));
        services.AddSingleton(x => ApplicationSettings);

        services.AddEntityFrameworkForApprenticeships(ApplicationSettings, Configuration.NotLocalOrAcceptanceTests());
        services.AddCommandServices(Configuration).AddDomainServices().AddQueryServices();

        services.AddLogging(builder =>
        {
            builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);

            builder.AddFilter(typeof(Program).Namespace, LogLevel.Information);
            builder.SetMinimumLevel(LogLevel.Trace);
        });

        services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights();
    }
}