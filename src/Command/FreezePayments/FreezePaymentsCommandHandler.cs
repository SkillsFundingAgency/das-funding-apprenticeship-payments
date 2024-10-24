using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.FreezePayments;

public class FreezePaymentsCommandHandler : ICommandHandler<FreezePaymentsCommand>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;

    public FreezePaymentsCommandHandler(IApprenticeshipRepository apprenticeshipRepository)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
    }

    public async Task Handle(FreezePaymentsCommand command)
    {
        var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
        apprenticeship.FreezePayments();
        await _apprenticeshipRepository.Update(apprenticeship);
    }
}