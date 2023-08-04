using System.Diagnostics.CodeAnalysis;
using NServiceBus;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

[ExcludeFromCodeCoverage]
public class DasServiceBusEndpoint : IDasServiceBusEndpoint
{
    private readonly EndpointConfiguration _endpointConfiguration;

    public DasServiceBusEndpoint(EndpointConfiguration endpointConfiguration)
    {
        _endpointConfiguration = endpointConfiguration;
    }

    public async Task Publish(object @event)
    {
        var endpointInstance = await Endpoint.Start(_endpointConfiguration).ConfigureAwait(false);
        await endpointInstance.Publish(@event);
    }
}