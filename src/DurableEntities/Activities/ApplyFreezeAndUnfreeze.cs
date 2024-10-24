using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ApplyFreezeAndUnfreeze;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;

public class ApplyFreezeAndUnfreeze
{
    private readonly ICommandHandler<ApplyFreezeAndUnfreezeCommand> _commandHandler;

    public ApplyFreezeAndUnfreeze(ICommandHandler<ApplyFreezeAndUnfreezeCommand> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    [FunctionName(nameof(ApplyFreezeAndUnfreeze))]
    public async Task Set([ActivityTrigger] ApplyFreezeAndUnfreezeInput input)
    {
        await _commandHandler.Handle(new ApplyFreezeAndUnfreezeCommand(input.ApprenticeshipKey, input.CollectionDetails.CollectionYear, input.CollectionDetails.CollectionPeriod));
    }
}