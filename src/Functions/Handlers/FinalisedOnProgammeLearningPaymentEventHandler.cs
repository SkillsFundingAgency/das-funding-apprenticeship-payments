using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Handlers;

public class FinalisedOnProgammeLearningPaymentEventHandler(ICommandHandler<CalculateRequiredLevyAmountCommand> commandHandler, ILogger<FinalisedOnProgammeLearningPaymentEventHandler> logger) : IHandleMessages<FinalisedOnProgammeLearningPaymentEvent>
{
    public async Task Handle(FinalisedOnProgammeLearningPaymentEvent message, IMessageHandlerContext context)
    {
        logger.LogInformation("Triggered {0} function for ApprenticeshipKey: {1}", nameof(FinalisedOnProgammeLearningPaymentEventHandler), message.ApprenticeshipKey);
        logger.LogInformation("ApprenticeshipKey: {0} Received FinalisedOnProgammeLearningPaymentEvent: {1}", message.ApprenticeshipKey, message.SerialiseForLogging());

        await commandHandler.Handle(new CalculateRequiredLevyAmountCommand(message));
    }
}