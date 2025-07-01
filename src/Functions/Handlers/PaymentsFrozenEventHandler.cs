using NServiceBus;
using SFA.DAS.Learning.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.FreezePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.UnfreezePayments;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Handlers;

public class PaymentsFrozenEventHandler(ICommandHandler<FreezePaymentsCommand> commandHandler, ILogger<PaymentsFrozenEventHandler> logger) : IHandleMessages<PaymentsFrozenEvent>
{
    public async Task Handle(PaymentsFrozenEvent message, IMessageHandlerContext context)
    {
        logger.LogInformation("Received PaymentsFrozenEvent for learning {learningKey}", message.LearningKey);

        var command = new FreezePaymentsCommand(message.LearningKey);
        await commandHandler.Handle(command);
    }
}

public class PaymentsUnfrozenEventHandler(ICommandHandler<UnfreezePaymentsCommand> commandHandler, ILogger<PaymentsUnfrozenEventHandler> logger) : IHandleMessages<PaymentsUnfrozenEvent>
{
    public async Task Handle(PaymentsUnfrozenEvent message, IMessageHandlerContext context)
    {
        logger.LogInformation("Received {eventName} for learning {LearningKey}", nameof(PaymentsUnfrozenEvent), message.LearningKey);

        var command = new UnfreezePaymentsCommand(message.LearningKey);
        await commandHandler.Handle(command);
    }
}