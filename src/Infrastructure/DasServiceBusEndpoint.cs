using System.Diagnostics.CodeAnalysis;
using NServiceBus;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

[ExcludeFromCodeCoverage]
public class DasServiceBusEndpoint : IDasServiceBusEndpoint
{
    private readonly IEndpointInstance _endpointInstance;

    public DasServiceBusEndpoint(IEndpointInstance endpointInstance)
    {
        _endpointInstance = endpointInstance;
    }

    public async Task Publish(object @event)
    {
        await _endpointInstance.Publish(@event);
    }
}