using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ReleasePayment;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;

public class ReleasePayment
{
    private readonly ILogger<ReleasePayment> _logger;
    private readonly ICommandHandler<ReleasePaymentCommand> _commandHandler;

    public ReleasePayment(
        ILogger<ReleasePayment> logger,
        ICommandHandler<ReleasePaymentCommand> commandHandler)
    {
        _logger = logger;
        _commandHandler = commandHandler;
    }

    [Function(nameof(ReleasePayment))]
    public async Task Set([ActivityTrigger] ReleasePaymentInput input)
    {
        using (_logger.BeginScope(input.GetLoggingScope()))
        {
            await _commandHandler.Handle(new ReleasePaymentCommand(input.ApprenticeshipKey, input.PaymentKey, input.CollectionDetails.CollectionYear, input.CollectionDetails.CollectionPeriod));
        }      
    }
}