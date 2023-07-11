using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using System;
using System.Threading.Tasks;

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

        [FunctionName(nameof(EarningsGeneratedEventHttpTrigger))]
        public async Task EarningsGeneratedEventHttpTrigger(
            [HttpTrigger(AuthorizationLevel.Function, "get")]HttpRequest request,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            var earningsGeneratedEvent = new EarningsGeneratedEvent { ApprenticeshipKey = Guid.NewGuid() };
            var entityId = new EntityId(nameof(ApprenticeshipEntity), earningsGeneratedEvent.ApprenticeshipKey.ToString());
            await client.SignalEntityAsync(entityId, nameof(ApprenticeshipEntity.HandleEarningsGeneratedEvent), earningsGeneratedEvent);
        }
    }
}