using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using SFA.DAS.ApprenticeshipPayments.Query;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions;

[ExcludeFromCodeCoverage]
public class Startup : FunctionsStartup
{
    public IConfiguration Configuration { get; set; }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();

        Configuration = GetConfiguration(serviceProvider);

        var applicationSettings = new ApplicationSettings();
        Configuration.Bind(nameof(ApplicationSettings), applicationSettings);
        EnsureConfig(applicationSettings);
        Environment.SetEnvironmentVariable("NServiceBusConnectionString", applicationSettings.NServiceBusConnectionString);

        builder.Services.Configure<ApprenticeshipsOuterApi>(Configuration.GetSection(nameof(ApprenticeshipsOuterApi)));
        builder.Services.AddSingleton(cfg => cfg.GetService<IOptions<ApprenticeshipsOuterApi>>()!.Value);

        builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), Configuration));
        builder.Services.AddSingleton(x => applicationSettings);

        builder.Services.AddNServiceBus(applicationSettings);
        builder.Services.AddEntityFrameworkForApprenticeships(applicationSettings, NotAcceptanceTests(Configuration));
        builder.Services.AddCommandServices(Configuration).AddDomainServices().AddQueryServices();
    }

    private static IConfiguration GetConfiguration(ServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetService<IConfiguration>();

        var configBuilder = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();

        if (NotAcceptanceTests(configuration))
        {
            configBuilder.AddJsonFile("local.settings.json", true);
            configBuilder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            });
        }

        return configBuilder.Build();
    }

    private static void EnsureConfig(ApplicationSettings applicationSettings)
    {
        if (string.IsNullOrWhiteSpace(applicationSettings.NServiceBusConnectionString))
            throw new InvalidOperationException("NServiceBusConnectionString in ApplicationSettings should not be null.");
    }

    private static bool NotAcceptanceTests(IConfiguration configuration)
    {
        return !configuration!["EnvironmentName"].Equals("LOCAL_ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase);
    }
}