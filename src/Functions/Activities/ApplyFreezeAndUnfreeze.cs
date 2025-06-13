using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ApplyFreezeAndUnfreeze;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;

public class ApplyFreezeAndUnfreeze
{
    private readonly ILogger<ApplyFreezeAndUnfreeze> _logger;
    private readonly ICommandHandler<ApplyFreezeAndUnfreezeCommand> _commandHandler;

    public ApplyFreezeAndUnfreeze(
        ILogger<ApplyFreezeAndUnfreeze> logger,
        ICommandHandler<ApplyFreezeAndUnfreezeCommand> commandHandler)
    {
        _logger = logger;
        _commandHandler = commandHandler;
    }

    [Function(nameof(ApplyFreezeAndUnfreeze))]
    public async Task Set([ActivityTrigger] ApplyFreezeAndUnfreezeInput input)
    {
        using (_logger.BeginScope(input.GetLoggingScope()))
        {
            await _commandHandler.Handle(new ApplyFreezeAndUnfreezeCommand(input.ApprenticeshipKey, input.CollectionDetails.CollectionYear, input.CollectionDetails.CollectionPeriod));
        }
    }
}