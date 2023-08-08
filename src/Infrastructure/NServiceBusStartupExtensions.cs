using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;
using SFA.DAS.NServiceBus.AzureFunction.Hosting;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Diagnostics.CodeAnalysis;

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

        ConfigureFundingServiceBus(serviceCollection, applicationSettings);
        ConfigurePv2ServiceBus(serviceCollection, applicationSettings);

        return serviceCollection;
    }

    private static void ConfigurePv2ServiceBus(IServiceCollection serviceCollection, ApplicationSettings applicationSettings)
    {
        var endpointConfiguration = new EndpointConfiguration("sfa.das.funding.payments")
                 .UseNewtonsoftJsonSerializer();
        endpointConfiguration.SendOnly();

        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningMessagesAs(type => type == typeof(CalculatedRequiredLevyAmount)); // Treat CalculatedRequiredLevyAmount as a message

        if (UsingLearningTransport(applicationSettings))
        {
            SetupLearningTransportEndpoint(endpointConfiguration);
        }
        else
        {
            endpointConfiguration
                .UseAzureServiceBusTransport(applicationSettings.DCServiceBusConnectionString,
                    r => r.AddRouting().DoNotEnforceBestPractices());
        }

        if (!string.IsNullOrEmpty(applicationSettings.NServiceBusLicense))
        {
            endpointConfiguration.License(applicationSettings.NServiceBusLicense);
        }

        var endpointInstance = Endpoint.Start(endpointConfiguration).ConfigureAwait(false).GetAwaiter().GetResult();
        serviceCollection.AddSingleton(typeof(IPaymentsV2ServiceBusEndpoint), new PaymentsV2ServiceBusEndpoint(endpointInstance));
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
            SetupLearningTransportEndpoint(endpointConfiguration);
        }
        else
        {
            endpointConfiguration.UseAzureServiceBusTransport(applicationSettings.NServiceBusConnectionString);
        }

        if (!string.IsNullOrEmpty(applicationSettings.NServiceBusLicense))
        {
            endpointConfiguration.License(applicationSettings.NServiceBusLicense);
        }

        var endpointInstance = Endpoint.Start(endpointConfiguration).ConfigureAwait(false).GetAwaiter().GetResult();
        serviceCollection.AddSingleton(typeof(IDasServiceBusEndpoint), new DasServiceBusEndpoint(endpointInstance));
    }

    private static bool UsingLearningTransport(ApplicationSettings applicationSettings)
    {
        return applicationSettings.NServiceBusConnectionString.Contains("UseLearningEndpoint=true", StringComparison.CurrentCultureIgnoreCase);
    }

    private static void SetupLearningTransportEndpoint(EndpointConfiguration endpointConfiguration)
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
}