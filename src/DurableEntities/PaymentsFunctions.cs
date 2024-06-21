using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;

public class PaymentsFunctions
{
    [FunctionName(nameof(ReleasePaymentsEventServiceBusTrigger))]
    public async Task ReleasePaymentsEventServiceBusTrigger(
        [NServiceBusTrigger(Endpoint = QueueNames.ReleasePayments)] ReleasePaymentsCommand releasePaymentsCommand,
        [DurableClient] IDurableEntityClient client,
        ILogger log)
    {
        using (var releasePaymentsMonitor = log.LogPerformance("ReleasePaymentsEventServiceBusTrigger"))
        {
            using CancellationTokenSource source = new CancellationTokenSource();
            var token = source.Token;
            var allApprenticeshipEntitiesQuery = new EntityQuery { EntityName = nameof(ApprenticeshipEntity) }; //default page size is 100, we may wish to tweak this in future to improve performance
            var pageCounter = 0;

            do
            {
                using (log.BeginScope("Loop CorrelationId: {loop_correlationId}", Guid.NewGuid()))
                {
                    pageCounter++;

                    var monitorListEntitiesAsync = log.LogPerformance("ListEntitiesAsync");

                    await client.CleanEntityStorageAsync(true, true, token);
                    var result = await client.ListEntitiesAsync(allApprenticeshipEntitiesQuery, token);
                    var releasePaymentsTasks = result.Entities.Select(x => client.SignalEntityAsync(x.EntityId, nameof(ApprenticeshipEntity.ReleasePaymentsForCollectionPeriod), releasePaymentsCommand));

                    monitorListEntitiesAsync.Dispose();

                    allApprenticeshipEntitiesQuery.ContinuationToken = result.ContinuationToken;

                    log.LogInformation($"Releasing payments for collection period {releasePaymentsCommand.CollectionPeriod} & year {releasePaymentsCommand.CollectionYear} for page {pageCounter} of entities. (Count: {result.Entities.Count()})");

                    using (var monitor = log.LogPerformance("Release Tasks"))
                    {
                        await Task.WhenAll(releasePaymentsTasks);
                    }
                }



            } while (allApprenticeshipEntitiesQuery.ContinuationToken != null);

            log.LogInformation($"Releasing payments for collection period {releasePaymentsCommand.CollectionPeriod} & year {releasePaymentsCommand.CollectionYear} complete.");
        }

    }
}