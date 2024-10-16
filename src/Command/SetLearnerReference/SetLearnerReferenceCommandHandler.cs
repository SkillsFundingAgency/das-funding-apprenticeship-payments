using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.SetLearnerReference;

public class SetLearnerReferenceCommandHandler : ISetLearnerReferenceCommandHandler
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly ILogger<CalculateApprenticeshipPaymentsCommandHandler> _logger;

    public SetLearnerReferenceCommandHandler(IApprenticeshipRepository apprenticeshipRepository, ILogger<CalculateApprenticeshipPaymentsCommandHandler> logger)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _logger = logger;
    }

    public async Task Set(SetLearnerReferenceCommand command)
    {
        var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
        if (apprenticeship == null)
        {
            _logger.LogCritical($"No apprenticeship with key {command.ApprenticeshipKey} found.");
            return;
        }

        apprenticeship.SetLearnerReference(command.LearnerReference);
        
        await _apprenticeshipRepository.Update(apprenticeship);
    }
}