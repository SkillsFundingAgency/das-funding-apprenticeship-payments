using SFA.DAS.Funding.ApprenticeshipPayments.Command.SetLearnerReference;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities
{
    public class SetLearnerReference
    {
        private readonly ISetLearnerReferenceCommandHandler _commandHandler;

        public SetLearnerReference(ISetLearnerReferenceCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        [FunctionName(nameof(SetLearnerReference))]
        public async Task Set([ActivityTrigger] object input)
        {
            var learner = (SetLearnerReferenceInput)input;
            await _commandHandler.Set(new SetLearnerReferenceCommand(learner.ApprenticeshipKey, learner.LearnerReference));
        }
    }
}
