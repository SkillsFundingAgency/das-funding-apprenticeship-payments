using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.SetLearnerReference;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;

public class SetLearnerReference
{
    private readonly ILogger<SetLearnerReference> _logger;
    private readonly ICommandHandler<SetLearnerReferenceCommand> _commandHandler;

    public SetLearnerReference(
        ILogger<SetLearnerReference> logger,
        ICommandHandler<SetLearnerReferenceCommand> commandHandler)
    {
        _logger = logger;
        _commandHandler = commandHandler;
    }

    [Function(nameof(SetLearnerReference))]
    public async Task Set([ActivityTrigger] SetLearnerReferenceInput input)
    {
        using (_logger.BeginScope(input.GetLoggingScope()))
        {
            await _commandHandler.Handle(new SetLearnerReferenceCommand(input.ApprenticeshipKey, input.LearnerReference));
        }    
    }
}