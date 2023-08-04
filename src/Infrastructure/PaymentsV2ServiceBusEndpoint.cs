using System.Diagnostics.CodeAnalysis;
using NServiceBus;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

[ExcludeFromCodeCoverage]
public class PaymentsV2ServiceBusEndpoint : IPaymentsV2ServiceBusEndpoint
{
    private readonly IStartableEndpointWithExternallyManagedContainer _endpointInstance;

    public PaymentsV2ServiceBusEndpoint(IStartableEndpointWithExternallyManagedContainer endpointInstance)
    {
        _endpointInstance = endpointInstance; 
    }

    public async Task Send(object message)
    {
        await _endpointInstance.MessageSession.Value.Send(QueueNames.CalculatedRequiredLevyAmount, message);
    }
}