using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;

[ExcludeFromCodeCoverage]
public class Startup : FunctionsStartup
{
    public IConfiguration Configuration { get; set; }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();

        var configuration = serviceProvider.GetService<IConfiguration>();

        var configBuilder = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();
            
        //if (NotAcceptanceTests(configuration))
        //{
            configBuilder.AddJsonFile("local.settings.json", true);
        //    configBuilder.AddAzureTableStorage(options =>
        //    {
        //        options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
        //        options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
        //        options.EnvironmentName = configuration["EnvironmentName"];
        //        options.PreFixConfigurationKeys = false;
        //    });
        //}

        Configuration = configBuilder.Build();

        var applicationSettings = new ApplicationSettings();
        Configuration.Bind(nameof(ApplicationSettings), applicationSettings);
        EnsureConfig(applicationSettings);
        Environment.SetEnvironmentVariable("NServiceBusConnectionString", applicationSettings.NServiceBusConnectionString);

        builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), Configuration));
        builder.Services.AddSingleton(x => applicationSettings);

        builder.Services.AddNServiceBus(applicationSettings);
        builder.Services.AddCommandServices().AddDomainServices();
    }

    private static void EnsureConfig(ApplicationSettings applicationSettings)
    {
        if (string.IsNullOrWhiteSpace(applicationSettings.NServiceBusConnectionString))
            throw new Exception("NServiceBusConnectionString in ApplicationSettings should not be null.");
    }

    private static bool NotAcceptanceTests(IConfiguration configuration)
    {
        return !configuration!["EnvironmentName"].Equals("LOCAL_ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase);
    }
}