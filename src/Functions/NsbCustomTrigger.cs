using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using NServiceBus;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions
{
    /// <summary>
    /// This replaces the NServiceBusTriggerFunction attribute, which is nerfed by any HttpTriggers in the same project
    /// </summary>
    /// <param name="functionEndpoint"></param>
    public class NsbCustomTrigger(IFunctionEndpoint functionEndpoint)
    {
        [Function("NServiceBusTriggerFunction")]
        public async Task Run(
            [ServiceBusTrigger("SFA.DAS.Funding.ApprenticeshipPayments", Connection = "AzureWebJobsServiceBus")]
            ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions, FunctionContext context,
            CancellationToken cancellationToken = default)
        {
            await functionEndpoint.Process(message, messageActions, context, cancellationToken);
        }
    }
}
