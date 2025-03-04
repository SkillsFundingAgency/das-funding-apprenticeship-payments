using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.FreezePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.UnfreezePayments;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions;

public class PaymentFreezeFunctions
{
    private readonly ICommandHandler<FreezePaymentsCommand> _freezePaymentsCommandHandler;
    private readonly ICommandHandler<UnfreezePaymentsCommand> _unfreezePaymentsCommandHandler;
    private readonly ILogger<PaymentFreezeFunctions> _logger;

    public PaymentFreezeFunctions(
        ICommandHandler<FreezePaymentsCommand> freezePaymentsCommandHandler,
        ICommandHandler<UnfreezePaymentsCommand> unfreezePaymentsCommandHandler,
        ILogger<PaymentFreezeFunctions> logger)
    {
        _freezePaymentsCommandHandler = freezePaymentsCommandHandler;
        _unfreezePaymentsCommandHandler = unfreezePaymentsCommandHandler;
        _logger = logger;
    }


    [Function(nameof(PaymentsFrozenEventServiceBusTrigger))]
    public async Task PaymentsFrozenEventServiceBusTrigger(
        [ServiceBusTrigger(QueueNames.PaymentsFrozen)] PaymentsFrozenEvent paymentsFrozenEvent)
    {
        _logger.LogInformation("Received PaymentsFrozenEvent for apprenticeship {apprenticeshipKey}", paymentsFrozenEvent.ApprenticeshipKey);

        var command = new FreezePaymentsCommand(paymentsFrozenEvent.ApprenticeshipKey);
        await _freezePaymentsCommandHandler.Handle(command);
    }

    [Function(nameof(PaymentsUnfrozenEventServiceBusTrigger))]
    public async Task PaymentsUnfrozenEventServiceBusTrigger(
        [ServiceBusTrigger(QueueNames.PaymentsUnfrozen)] PaymentsUnfrozenEvent paymentsUnfrozenEvent)
    {
        _logger.LogInformation("Received {eventName} for apprenticeship {apprenticeshipKey}", nameof(PaymentsUnfrozenEvent), paymentsUnfrozenEvent.ApprenticeshipKey);

        var command = new UnfreezePaymentsCommand(paymentsUnfrozenEvent.ApprenticeshipKey);
        await _unfreezePaymentsCommandHandler.Handle(command);
    }
}
