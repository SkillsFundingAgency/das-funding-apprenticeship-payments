using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.FreezePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.UnfreezePayments;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions;

public class PaymentFreezeFunctions
{
    private IFreezePaymentsCommandHandler _freezePaymentsCommandHandler;
    private readonly IUnfreezePaymentsCommandHandler _unfreezePaymentsCommandHandler;

    public PaymentFreezeFunctions(IFreezePaymentsCommandHandler freezePaymentsCommandHandler, IUnfreezePaymentsCommandHandler unfreezePaymentsCommandHandler)
    {
        _freezePaymentsCommandHandler = freezePaymentsCommandHandler;
        _unfreezePaymentsCommandHandler = unfreezePaymentsCommandHandler;
    }


    [FunctionName(nameof(PaymentsFrozenEventServiceBusTrigger))]
    public async Task PaymentsFrozenEventServiceBusTrigger(
        [NServiceBusTrigger(Endpoint = QueueNames.PaymentsFrozen)] PaymentsFrozenEvent paymentsFrozenEvent,
        ILogger log)
    {
        log.LogInformation("Received PaymentsFrozenEvent for apprenticeship {apprenticeshipKey}", paymentsFrozenEvent.ApprenticeshipKey);

        var command = new FreezePaymentsCommand(paymentsFrozenEvent.ApprenticeshipKey);
        await _freezePaymentsCommandHandler.Freeze(command);
    }

    [FunctionName(nameof(PaymentsUnfrozenEventServiceBusTrigger))]
    public async Task PaymentsUnfrozenEventServiceBusTrigger(
        [NServiceBusTrigger(Endpoint = QueueNames.PaymentsUnfrozen)] PaymentsUnfrozenEvent paymentsUnfrozenEvent,
        [DurableClient] IDurableEntityClient client,
        ILogger log)
    {
        log.LogInformation("Received {eventName} for apprenticeship {apprenticeshipKey}", nameof(PaymentsUnfrozenEvent), paymentsUnfrozenEvent.ApprenticeshipKey);

        var command = new UnfreezePaymentsCommand(paymentsUnfrozenEvent.ApprenticeshipKey);
        await _unfreezePaymentsCommandHandler.Unfreeze(command);
    }
}
