using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities
{
    public class EarningsFunctions
    {
        [FunctionName(nameof(EarningsGeneratedEventServiceBusTrigger))]
        public async Task EarningsGeneratedEventServiceBusTrigger(
            [NServiceBusTrigger(Endpoint = QueueNames.EarningsGenerated)] EarningsGeneratedEvent earningsGeneratedEvent,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            var entityId = new EntityId(nameof(ApprenticeshipEntity), earningsGeneratedEvent.ApprenticeshipKey.ToString());
            await client.SignalEntityAsync(entityId, nameof(ApprenticeshipEntity.HandleEarningsGeneratedEvent), earningsGeneratedEvent);
        }
    }
}