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
        log.LogInformation("Received PaymentsFrozenEvent for apprenticeship {apprenticeshipKey}", paymentsFrozenEvent.ApprenticeshipKey);

        var entityId = new EntityId(nameof(ApprenticeshipEntity), paymentsFrozenEvent.ApprenticeshipKey.ToString());
        await client.SignalEntityAsync(entityId, nameof(ApprenticeshipEntity.HandlePaymentFrozenEvent), paymentsFrozenEvent);
    }

    [FunctionName(nameof(PaymentsUnfrozenEventServiceBusTrigger))]
    public async Task PaymentsUnfrozenEventServiceBusTrigger(
        [NServiceBusTrigger(Endpoint = QueueNames.PaymentsUnfrozen)] PaymentsUnfrozenEvent paymentsUnfrozenEvent,
        [DurableClient] IDurableEntityClient client,
        ILogger log)
    {
        log.LogInformation("Received {eventName} for apprenticeship {apprenticeshipKey}", nameof(PaymentsUnfrozenEvent), paymentsUnfrozenEvent.ApprenticeshipKey);

        var entityId = new EntityId(nameof(ApprenticeshipEntity), paymentsUnfrozenEvent.ApprenticeshipKey.ToString());
        await client.SignalEntityAsync(entityId, nameof(ApprenticeshipEntity.HandlePaymentsUnfrozenEvent), paymentsUnfrozenEvent);
    }
}
