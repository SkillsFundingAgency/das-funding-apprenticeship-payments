using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnfreezePayments
{
    public class UnfreezePaymentsCommandHandler : IUnfreezePaymentsCommandHandler
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;

        public UnfreezePaymentsCommandHandler(IApprenticeshipRepository apprenticeshipRepository)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
        }

        public async Task Unfreeze(UnfreezePaymentsCommand command)
        {
            var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
            apprenticeship.UnfreezePayments();
            await _apprenticeshipRepository.Update(apprenticeship);
        }
    }
}
