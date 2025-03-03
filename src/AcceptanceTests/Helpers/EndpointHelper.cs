﻿using NServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;

public static class EndpointHelper
{
    public static string EventStorageFolder => Path.Combine(Directory.GetCurrentDirectory()[..Directory.GetCurrentDirectory().IndexOf("src", StringComparison.Ordinal)], @"src\.learningtransport");
        
    public static async Task<IEndpointInstance?> StartEndpoint(string endpointName, bool isSendOnly, Type[] types)
    {
        var endpointConfiguration = new EndpointConfiguration(endpointName);
        endpointConfiguration.AssemblyScanner().ThrowExceptions = false;

        if (isSendOnly) endpointConfiguration.SendOnly();

        endpointConfiguration.UseNewtonsoftJsonSerializer();
        endpointConfiguration.Conventions().DefiningEventsAs(types.Contains);

        var transport = endpointConfiguration.UseTransport<LearningTransport>();

        var eventStorageFolder = Path.Combine(Directory.GetCurrentDirectory()[..Directory.GetCurrentDirectory().IndexOf("src", StringComparison.Ordinal)], @"src\.learningtransport");
        transport.StorageDirectory(eventStorageFolder);

        return await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
    }

    public static void ClearEventStorage()
    {
        var di = new DirectoryInfo(EventStorageFolder);
        if (!di.Exists) return;

        foreach (var file in di.GetFiles())
        {
            file.Delete();
        }

        foreach (var dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }
}