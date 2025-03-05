using Microsoft.DurableTask.Client;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Orchestrators;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Handlers
{
    public class ReleasePaymentsCommandHandler(DurableTaskClient client, ILogger<ReleasePaymentsCommand> logger) : IHandleMessages<ReleasePaymentsCommand>
    {
        public async Task Handle(ReleasePaymentsCommand message, IMessageHandlerContext context)
        {
            logger.LogInformation("Handling ReleasePaymentCommand");


            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(ReleasePaymentsOrchestrator),
                input: new CollectionDetails(message.CollectionPeriod, message.CollectionYear));

            logger.LogInformation($"Scheduled new orchestration instance Id {instanceId}");
        }
    }
}
