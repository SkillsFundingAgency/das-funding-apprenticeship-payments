using System.Diagnostics.CodeAnalysis;
using NServiceBus;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

[ExcludeFromCodeCoverage]
public class DasServiceBusEndpoint : IDasServiceBusEndpoint
{
    private readonly IStartableEndpointWithExternallyManagedContainer _endpointInstance;

    public DasServiceBusEndpoint(IStartableEndpointWithExternallyManagedContainer endpointInstance)
    {
        _endpointInstance = endpointInstance;
    }

    public async Task Publish(object @event)
    {
        await _endpointInstance.MessageSession.Value.Publish(@event);
    }
}