using SFA.DAS.Funding.ApprenticeshipPayments.Command.ApplyFreezeAndUnfreeze;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities
{
    public class ApplyFreezeAndUnfreeze
    {
        private readonly IApplyFreezeAndUnfreezeCommandHandler _commandHandler;

        public ApplyFreezeAndUnfreeze(IApplyFreezeAndUnfreezeCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        [FunctionName(nameof(ApplyFreezeAndUnfreeze))]
        public async Task Set([ActivityTrigger] ApplyFreezeAndUnfreezeInput input)
        {
            await _commandHandler.Apply(new ApplyFreezeAndUnfreezeCommand(input.ApprenticeshipKey, input.CollectionDetails.CollectionYear, input.CollectionDetails.CollectionPeriod));
        }
    }
}
