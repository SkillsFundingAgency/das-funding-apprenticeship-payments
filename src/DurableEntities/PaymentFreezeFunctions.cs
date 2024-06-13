using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;

public class PaymentFreezeFunctions
{

    [FunctionName(nameof(PaymentsFrozenEventServiceBusTrigger))]
    public async Task PaymentsFrozenEventServiceBusTrigger(
        [NServiceBusTrigger(Endpoint = QueueNames.PaymentsFrozen)] PaymentsFrozenEvent paymentsFrozenEvent,
        [DurableClient] IDurableEntityClient client,
        ILogger log)
    {
        log.LogInformation($"Received PaymentsFrozenEvent for apprenticeship {paymentsFrozenEvent.ApprenticeshipKey}");

        var entityId = new EntityId(nameof(ApprenticeshipEntity), paymentsFrozenEvent.ApprenticeshipKey.ToString());
        await client.SignalEntityAsync(entityId, nameof(ApprenticeshipEntity.HandlePaymentFrozenEvent), paymentsFrozenEvent);
    }
}
