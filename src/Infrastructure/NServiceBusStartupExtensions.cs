using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;
using SFA.DAS.NServiceBus.AzureFunction.Hosting;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

[ExcludeFromCodeCoverage]
public static class NServiceBusStartupExtensions
{
    public static IServiceCollection AddNServiceBus(
        this IServiceCollection serviceCollection,
        ApplicationSettings applicationSettings)
    {
        var webBuilder = serviceCollection.AddWebJobs(_ => { });
        webBuilder.AddExecutionContextBinding();
        webBuilder.AddExtension(new NServiceBusExtensionConfigProvider());

        ConfigurePv2ServiceBus(serviceCollection, applicationSettings);
        ConfigureFundingServiceBus(serviceCollection, applicationSettings);

        return serviceCollection;
    }

    private static void ConfigurePv2ServiceBus(IServiceCollection serviceCollection, ApplicationSettings applicationSettings)
    {
        var endpointConfiguration = new EndpointConfiguration("sfa.das.funding.payments.calculatedlevyamount")
                 .UseNewtonsoftJsonSerializer();
        endpointConfiguration.SendOnly();

        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningMessagesAs(type => type == typeof(CalculatedRequiredLevyAmount)); // Treat CalculatedRequiredLevyAmount as a message

        if (UsingLearningTransport(applicationSettings))
        {
            SetupLearningTrasportEndpoint(endpointConfiguration);
        }
        else
        {
            endpointConfiguration
                .UseAzureServiceBusTransport(applicationSettings.NServiceBusConnectionString,
                // TODO: .UseAzureServiceBusTransport(applicationSettings.DCServiceBusConnectionString,
                    r => r.AddRouting().DoNotEnforceBestPractices());
        }

        if (!string.IsNullOrEmpty(applicationSettings.NServiceBusLicense))
        {
            endpointConfiguration.License(applicationSettings.NServiceBusLicense);
        }

        var paymentsV2EndpointWithExternallyManagedServiceProvider = EndpointWithExternallyManagedServiceProvider.Create(endpointConfiguration, serviceCollection);
        paymentsV2EndpointWithExternallyManagedServiceProvider.Start(new UpdateableServiceProvider(serviceCollection));

        serviceCollection.AddSingleton(typeof(IPaymentsV2ServiceBusEndpoint), new PaymentsV2ServiceBusEndpoint(paymentsV2EndpointWithExternallyManagedServiceProvider));
    }

    private static void ConfigureFundingServiceBus(IServiceCollection serviceCollection, ApplicationSettings applicationSettings)
    {
        var endpointConfiguration = new EndpointConfiguration("SFA.DAS.Funding.ApprenticeshipPayments")
            .UseMessageConventions()
            .UseNewtonsoftJsonSerializer();

        endpointConfiguration.SendOnly();
        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningMessagesAs(type => type == typeof(CalculatedRequiredLevyAmount)); // Treat CalculatedRequiredLevyAmount as a message

        if (UsingLearningTransport(applicationSettings))
        {
            SetupLearningTrasportEndpoint(endpointConfiguration);
        }
        else
        {
            endpointConfiguration.UseAzureServiceBusTransport(applicationSettings.NServiceBusConnectionString);
        }

        if (!string.IsNullOrEmpty(applicationSettings.NServiceBusLicense))
        {
            endpointConfiguration.License(applicationSettings.NServiceBusLicense);
        }

        ExcludeTestAssemblies(endpointConfiguration.AssemblyScanner());

        var endpointWithExternallyManagedServiceProvider = EndpointWithExternallyManagedServiceProvider.Create(endpointConfiguration, serviceCollection);
        endpointWithExternallyManagedServiceProvider.Start(new UpdateableServiceProvider(serviceCollection));
        serviceCollection.AddSingleton(p => endpointWithExternallyManagedServiceProvider.MessageSession.Value);
    }

    private static bool UsingLearningTransport(ApplicationSettings applicationSettings)
    {
        return applicationSettings.NServiceBusConnectionString.Equals("UseLearningEndpoint=true", StringComparison.CurrentCultureIgnoreCase);
    }

    private static void SetupLearningTrasportEndpoint(EndpointConfiguration endpointConfiguration)
    {
        var learningTransportFolder =
            Path.Combine(
                Directory.GetCurrentDirectory()[..Directory.GetCurrentDirectory().IndexOf("src", StringComparison.Ordinal)],
                @"src\.learningtransport");
        endpointConfiguration
            .UseTransport<LearningTransport>()
            .StorageDirectory(learningTransportFolder);
        endpointConfiguration.UseLearningTransport(r => r.AddRouting().DoNotEnforceBestPractices());
        Environment.SetEnvironmentVariable("LearningTransportStorageDirectory", learningTransportFolder, EnvironmentVariableTarget.Process);
    }

    private static void ExcludeTestAssemblies(AssemblyScannerConfiguration scanner)
    {
        var excludeRegexs = new List<string> { @"nunit.*.dll" };

        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        foreach (var fileName in Directory.EnumerateFiles(baseDirectory, "*.dll")
                     .Select(Path.GetFileName))
        {
            if (fileName != null && excludeRegexs.Any(pattern => Regex.IsMatch(fileName, pattern, RegexOptions.IgnoreCase)))
            {
                scanner.ExcludeAssemblies(fileName);
            }
        }
    }
}