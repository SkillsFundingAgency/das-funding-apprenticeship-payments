using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ReleasePayment;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;

public class ReleasePayment
{
    private readonly ICommandHandler<ReleasePaymentCommand> _commandHandler;

    public ReleasePayment(ICommandHandler<ReleasePaymentCommand> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    [FunctionName(nameof(ReleasePayment))]
    public async Task Set([ActivityTrigger] ReleasePaymentInput input)
    {
        await _commandHandler.Handle(new ReleasePaymentCommand(input.ApprenticeshipKey, input.PaymentKey));
    }
}