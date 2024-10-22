using SFA.DAS.Funding.ApprenticeshipPayments.Command.ReleasePayment;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities
{
    public class ReleasePayment
    {
        private readonly IReleasePaymentCommandHandler _commandHandler;

        public ReleasePayment(IReleasePaymentCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        [FunctionName(nameof(ReleasePayment))]
        public async Task Set([ActivityTrigger] ReleasePaymentInput input)
        {
            await _commandHandler.Release(new ReleasePaymentCommand(input.ApprenticeshipKey, input.PaymentKey));
        }
    }
}
