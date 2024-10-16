using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetApprenticeshipKey
{
    public class GetApprenticeshipKeyQueryHandler : IGetApprenticeshipKeyQueryHandler
    {
        private IApprenticeshipQueryRepository _repository;

        public GetApprenticeshipKeyQueryHandler(IApprenticeshipQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetApprenticeshipKeyResponse> Get(GetApprenticeshipKeyQuery command)
        {
            var apprenticeshipKey = await _repository.GetApprenticeshipKey(command.Ukprn, command.Uln);
            return new GetApprenticeshipKeyResponse(apprenticeshipKey);
        }
    }
}
