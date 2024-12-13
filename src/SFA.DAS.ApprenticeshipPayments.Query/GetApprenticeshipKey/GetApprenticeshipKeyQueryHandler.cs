using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetApprenticeshipKey;

public class GetApprenticeshipKeyQueryHandler : IQueryHandler<GetApprenticeshipKeyResponse, GetApprenticeshipKeyQuery>
{
    private IApprenticeshipQueryRepository _repository;

    public GetApprenticeshipKeyQueryHandler(IApprenticeshipQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetApprenticeshipKeyResponse> Get(GetApprenticeshipKeyQuery query)
    {
        var apprenticeshipKey = await _repository.GetApprenticeshipKey(query.Ukprn, query.Uln);
        return new GetApprenticeshipKeyResponse(apprenticeshipKey);
    }
}