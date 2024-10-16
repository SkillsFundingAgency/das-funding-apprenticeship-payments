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
        public async Task Set([ActivityTrigger] object input)
        {
            var applyFreezeAndUnfreezeInput = (ApplyFreezeAndUnfreezeInput)input;
            await _commandHandler.Apply(new ApplyFreezeAndUnfreezeCommand(applyFreezeAndUnfreezeInput.ApprenticeshipKey, applyFreezeAndUnfreezeInput.CollectionDetails.CollectionYear, applyFreezeAndUnfreezeInput.CollectionDetails.CollectionPeriod));
        }
    }
}
