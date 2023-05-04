using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;

public class PaymentsFunctions
{
    [FunctionName(nameof(ReleasePaymentsEventServiceBusTrigger))]
    public async Task ReleasePaymentsEventServiceBusTrigger(
        [NServiceBusTrigger(Endpoint = QueueNames.ReleasePayments)] ReleasePaymentsCommand releasePaymentsCommand,
        [DurableClient] IDurableEntityClient client,
        ILogger log)
    {
        var allApprenticeshipEntitiesQuery = await client.ListEntitiesAsync(new EntityQuery{ EntityName = nameof(ApprenticeshipEntity)}, CancellationToken.None);
        log.LogInformation($"Releasing payments for collection month 11 for all {allApprenticeshipEntitiesQuery.Entities.Count()} entities.");
        var releasePaymentsTasks = allApprenticeshipEntitiesQuery.Entities.Select(x => client.SignalEntityAsync(x.EntityId, nameof(ApprenticeshipEntity.ReleasePaymentsForCollectionMonth), releasePaymentsCommand.CollectionMonth));
        await Task.WhenAll(releasePaymentsTasks);
    }
}