using NServiceBus;
using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.FreezePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.UnfreezePayments;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Handlers;

public class PaymentsFrozenEventHandler(ICommandHandler<FreezePaymentsCommand> commandHandler, ILogger<PaymentsFrozenEventHandler> logger) : IHandleMessages<PaymentsFrozenEvent>
{
    public async Task Handle(PaymentsFrozenEvent message, IMessageHandlerContext context)
    {
        logger.LogInformation("Received PaymentsFrozenEvent for apprenticeship {apprenticeshipKey}", message.ApprenticeshipKey);

        var command = new FreezePaymentsCommand(message.ApprenticeshipKey);
        await commandHandler.Handle(command);
    }
}

public class PaymentsUnfrozenEventHandler(ICommandHandler<UnfreezePaymentsCommand> commandHandler, ILogger<PaymentsUnfrozenEventHandler> logger) : IHandleMessages<PaymentsUnfrozenEvent>
{
    public async Task Handle(PaymentsUnfrozenEvent message, IMessageHandlerContext context)
    {
        logger.LogInformation("Received {eventName} for apprenticeship {apprenticeshipKey}", nameof(PaymentsUnfrozenEvent), message.ApprenticeshipKey);

        var command = new UnfreezePaymentsCommand(message.ApprenticeshipKey);
        await commandHandler.Handle(command);
    }
}