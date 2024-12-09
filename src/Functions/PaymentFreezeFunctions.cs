using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.FreezePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.UnfreezePayments;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions;

public class PaymentFreezeFunctions
{
    private readonly ICommandHandler<FreezePaymentsCommand> _freezePaymentsCommandHandler;
    private readonly ICommandHandler<UnfreezePaymentsCommand> _unfreezePaymentsCommandHandler;

    public PaymentFreezeFunctions(ICommandHandler<FreezePaymentsCommand> freezePaymentsCommandHandler, ICommandHandler<UnfreezePaymentsCommand> unfreezePaymentsCommandHandler)
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
        await _freezePaymentsCommandHandler.Handle(command);
    }

    [FunctionName(nameof(PaymentsUnfrozenEventServiceBusTrigger))]
    public async Task PaymentsUnfrozenEventServiceBusTrigger(
        [NServiceBusTrigger(Endpoint = QueueNames.PaymentsUnfrozen)] PaymentsUnfrozenEvent paymentsUnfrozenEvent,
        [DurableClient] IDurableEntityClient client,
        ILogger log)
    {
        log.LogInformation("Received {eventName} for apprenticeship {apprenticeshipKey}", nameof(PaymentsUnfrozenEvent), paymentsUnfrozenEvent.ApprenticeshipKey);

        var command = new UnfreezePaymentsCommand(paymentsUnfrozenEvent.ApprenticeshipKey);
        await _unfreezePaymentsCommandHandler.Handle(command);
    }
}
