using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;
using SFA.DAS.NServiceBus.AzureFunction.Hosting;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NServiceBus.Routing;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

[ExcludeFromCodeCoverage]
public static class NServiceBusStartupExtensions
{
    public static IServiceCollection AddNServiceBus(
        this IServiceCollection serviceCollection,
        ApplicationSettings applicationSettings)
    {
        var webBuilder = serviceCollection.AddWebJobs(x => { });
        webBuilder.AddExecutionContextBinding();
        webBuilder.AddExtension(new NServiceBusExtensionConfigProvider());

        ConfigurePV2ServiceBus(serviceCollection, applicationSettings);
        ConfigureServiceBus(serviceCollection, applicationSettings);

        return serviceCollection;
    }

    private static void ConfigurePV2ServiceBus(IServiceCollection serviceCollection, ApplicationSettings applicationSettings)
    {
        var endpointConfiguration = new EndpointConfiguration("sfa.das.funding.payments.calculatedlevyamount")
                // .UseMessageConventions()
                 .UseNewtonsoftJsonSerializer();

        endpointConfiguration.SendOnly();

        endpointConfiguration.Conventions().DefiningEventsAs(type => type.IsEvent<IPaymentsEvent>());

        if (applicationSettings.NServiceBusConnectionString.Equals("UseLearningEndpoint=true", StringComparison.CurrentCultureIgnoreCase))
        {
            var learningTransportFolder =
                Path.Combine(
                    Directory.GetCurrentDirectory()[..Directory.GetCurrentDirectory().IndexOf("src", StringComparison.Ordinal)],
                    @"src\.learningtransport");
            endpointConfiguration
                .UseTransport<LearningTransport>()
                .StorageDirectory(learningTransportFolder);
            endpointConfiguration.UseLearningTransport();
            Environment.SetEnvironmentVariable("LearningTransportStorageDirectory", learningTransportFolder, EnvironmentVariableTarget.Process);
        }
        else
        {
            endpointConfiguration
                //.UseAzureServiceBusTransport(applicationSettings.DCServiceBusConnectionString); TODO: Setup config
                .UseAzureServiceBusTransport(applicationSettings.NServiceBusConnectionString, r => r.AddRouting());
        }

        if (!string.IsNullOrEmpty(applicationSettings.NServiceBusLicense))
        {
            endpointConfiguration.License(applicationSettings.NServiceBusLicense);
        }


       // var endpointInstance = Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

        var paymentsV2EndpointWithExternallyManagedServiceProvider = EndpointWithExternallyManagedServiceProvider.Create(endpointConfiguration, serviceCollection);
        paymentsV2EndpointWithExternallyManagedServiceProvider.Start(new UpdateableServiceProvider(serviceCollection));

        serviceCollection.AddSingleton(typeof(IPaymentsV2ServiceBusEndpoint), new PaymentsV2ServiceBusEndpoint(paymentsV2EndpointWithExternallyManagedServiceProvider));
    }

    private static void ConfigureServiceBus(IServiceCollection serviceCollection, ApplicationSettings applicationSettings)
    {
        var endpointConfiguration = new EndpointConfiguration("SFA.DAS.Funding.ApprenticeshipPayments")
            .UseMessageConventions()
            .UseNewtonsoftJsonSerializer();

        endpointConfiguration.SendOnly();

        if (applicationSettings.NServiceBusConnectionString.Equals("UseLearningEndpoint=true", StringComparison.CurrentCultureIgnoreCase))
        {
            var learningTransportFolder =
                Path.Combine(
                    Directory.GetCurrentDirectory()[..Directory.GetCurrentDirectory().IndexOf("src", StringComparison.Ordinal)],
                    @"src\.learningtransport");
            endpointConfiguration
                .UseTransport<LearningTransport>()
                .StorageDirectory(learningTransportFolder);
            endpointConfiguration.UseLearningTransport();
            Environment.SetEnvironmentVariable("LearningTransportStorageDirectory", learningTransportFolder, EnvironmentVariableTarget.Process);
        }
        else
        {
            endpointConfiguration
                .UseAzureServiceBusTransport(applicationSettings.NServiceBusConnectionString);
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

    private static void ExcludeTestAssemblies(AssemblyScannerConfiguration scanner)
    {
        var excludeRegexs = new List<string>
        {
            @"nunit.*.dll"
        };

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

[ExcludeFromCodeCoverage]
public class PaymentsV2ServiceBusEndpoint : IPaymentsV2ServiceBusEndpoint
{
    private readonly IStartableEndpointWithExternallyManagedContainer _endpointInstance;

    public PaymentsV2ServiceBusEndpoint(IStartableEndpointWithExternallyManagedContainer endpointInstance)
    {
        _endpointInstance = endpointInstance;
    }

    public async Task Publish(object message)
    {
        await _endpointInstance.MessageSession.Value.Publish(message);
    }
}

public interface IPaymentsV2ServiceBusEndpoint
{
    public Task Publish(object message);
}