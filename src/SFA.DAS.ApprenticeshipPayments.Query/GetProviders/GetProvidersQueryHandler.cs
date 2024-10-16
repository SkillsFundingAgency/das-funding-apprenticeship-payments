using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetProviders
{
    public class GetProvidersQueryHandler : IGetProvidersQueryHandler
    {
        private IApprenticeshipQueryRepository _repository;

        public GetProvidersQueryHandler(IApprenticeshipQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetProvidersResponse> Get(GetProvidersQuery command)
        {
            var providers = await _repository.GetAllProviders();
            return new GetProvidersResponse(providers.Select(x => new Provider(x)));
        }
    }
}
