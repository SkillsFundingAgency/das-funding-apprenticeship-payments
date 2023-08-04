using System.Diagnostics.CodeAnalysis;
using NServiceBus;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

[ExcludeFromCodeCoverage]
public class PaymentsV2ServiceBusEndpoint : IPaymentsV2ServiceBusEndpoint
{
    private readonly IEndpointInstance _endpointInstance;

    public PaymentsV2ServiceBusEndpoint(IEndpointInstance endpointInstance)
    {
        _endpointInstance = endpointInstance; 
    }

    public async Task Send(object message)
    {
        await _endpointInstance.Send(QueueNames.CalculatedRequiredLevyAmount, message);
    }
}