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
        using CancellationTokenSource source = new CancellationTokenSource();
        var token = source.Token;
        var allApprenticeshipEntitiesQuery = new EntityQuery { EntityName = nameof(ApprenticeshipEntity) }; //default page size is 100, we may wish to tweak this in future to improve performance
        var pageCounter = 0;

        do
        {
            pageCounter++;
            var result = await client.ListEntitiesAsync(allApprenticeshipEntitiesQuery, token);
            var releasePaymentsTasks = result.Entities.Select(x => client.SignalEntityAsync(x.EntityId, nameof(ApprenticeshipEntity.ReleasePaymentsForCollectionMonth), releasePaymentsCommand.CollectionMonth));

            allApprenticeshipEntitiesQuery.ContinuationToken = result.ContinuationToken;

            log.LogInformation($"Releasing payments for collection month {releasePaymentsCommand.CollectionMonth} for page {pageCounter} of entities. (Count: {result.Entities.Count()})");
            await Task.WhenAll(releasePaymentsTasks);

        } while (allApprenticeshipEntitiesQuery.ContinuationToken != null);

        log.LogInformation($"Releasing payments for collection month {releasePaymentsCommand.CollectionMonth} complete.");
    }
}