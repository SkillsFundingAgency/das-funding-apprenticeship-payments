using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.FreezePayments
{
    public class FreezePaymentsCommandHandler : IFreezePaymentsCommandHandler
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;

        public FreezePaymentsCommandHandler(IApprenticeshipRepository apprenticeshipRepository)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
        }

        public async Task Freeze(FreezePaymentsCommand command)
        {
            var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
            apprenticeship.FreezePayments();
            await _apprenticeshipRepository.Update(apprenticeship);
        }
    }
}
