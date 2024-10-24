using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions;

public class CalculateRequiredLevyAmountFunction
{
    private readonly ICommandHandler<CalculateRequiredLevyAmountCommand> _commandHandler;

    public CalculateRequiredLevyAmountFunction(ICommandHandler<CalculateRequiredLevyAmountCommand> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    [FunctionName(nameof(CalculateRequiredLevyAmountFunction))]
    public async Task Run(
        [NServiceBusTrigger(Endpoint = QueueNames.FinalisedOnProgammeLearningPayment)] FinalisedOnProgammeLearningPaymentEvent @event,
        ILogger log)
    {
        log.LogInformation("Triggered {0} function for ApprenticeshipKey: {1}", nameof(CalculateRequiredLevyAmountFunction), @event.ApprenticeshipKey);
        log.LogInformation("ApprenticeshipKey: {0} Received FinalisedOnProgammeLearningPaymentEvent: {1}", @event.ApprenticeshipKey, @event.SerialiseForLogging());

        await _commandHandler.Handle(new CalculateRequiredLevyAmountCommand(@event));
    }
}