﻿using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.HealthChecks;

[ExcludeFromCodeCoverage]
internal class ServiceBusReceiveHealthCheck : BaseHealthCheck<ServiceBusReceiveHealthCheck>
{
    private readonly string _connectionString;
    private readonly string _serviceBusDisplayName;

    internal ServiceBusReceiveHealthCheck(string connectionString, string serviceBusDisplayName, ILogger<ServiceBusReceiveHealthCheck> logger) : base(logger)
    {
        _connectionString = connectionString;
        _serviceBusDisplayName = serviceBusDisplayName;
    }

    protected override async Task<HealthCheckResult> HealthCheck(CancellationToken cancellationToken)
    {
        try
        {
            var credential = new DefaultAzureCredential();
            await using var client = new ServiceBusClient(_connectionString.GetFullyQualifiedNamespace(), credential);
            var receiver = client.CreateReceiver(Constants.EndpointName);

            // Peek a message non-destructively to verify connectivity
            var message = await receiver.PeekMessageAsync(cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy($"Connected to {_serviceBusDisplayName} Azure Service Bus.");
        }
        catch (Exception ex)
        {
            LogError("Azure Service Bus 'Receive' failed.", ex);
            return HealthCheckResult.Unhealthy("Azure Service Bus 'Receive' failed.");
        }
    }
}
