using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions;

public class CalculateRequiredLevyAmountFunction
{
    private readonly ICommandHandler<CalculateRequiredLevyAmountCommand> _commandHandler;
    private readonly ILogger<CalculateRequiredLevyAmountFunction> _logger;

    public CalculateRequiredLevyAmountFunction(ICommandHandler<CalculateRequiredLevyAmountCommand> commandHandler, ILogger<CalculateRequiredLevyAmountFunction> logger)
    {
        _commandHandler = commandHandler;
        _logger = logger;
    }

    [Function(nameof(CalculateRequiredLevyAmountFunction))]
    public async Task Run(
        [ServiceBusTrigger(QueueNames.FinalisedOnProgammeLearningPayment)] FinalisedOnProgammeLearningPaymentEvent @event)
    {
        _logger.LogInformation("Triggered {0} function for ApprenticeshipKey: {1}", nameof(CalculateRequiredLevyAmountFunction), @event.ApprenticeshipKey);
        _logger.LogInformation("ApprenticeshipKey: {0} Received FinalisedOnProgammeLearningPaymentEvent: {1}", @event.ApprenticeshipKey, @event.SerialiseForLogging());

        await _commandHandler.Handle(new CalculateRequiredLevyAmountCommand(@event));
    }
}