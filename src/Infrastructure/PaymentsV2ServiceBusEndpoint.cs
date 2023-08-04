using System.Diagnostics.CodeAnalysis;
using NServiceBus;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

[ExcludeFromCodeCoverage]
public class PaymentsV2ServiceBusEndpoint : IPaymentsV2ServiceBusEndpoint
{
    private readonly EndpointConfiguration _endpointConfiguration;

    public PaymentsV2ServiceBusEndpoint(EndpointConfiguration endpointConfiguration)
    {
        _endpointConfiguration = endpointConfiguration;
    }

    public async Task Send(object message)
    {
        var endpointInstance = await Endpoint.Start(_endpointConfiguration).ConfigureAwait(false);
         
        await endpointInstance.Send(QueueNames.CalculatedRequiredLevyAmount, message);
    }
}