﻿using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;
using SFA.DAS.NServiceBus.AzureFunction.Hosting;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;

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

        return serviceCollection;
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