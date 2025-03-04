using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Dtos;
using SFA.DAS.Funding.ApprenticeshipPayments.Query;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetApprenticeshipKey;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;

public class GetApprenticeshipKey
{
    private readonly IQueryHandler<GetApprenticeshipKeyResponse, GetApprenticeshipKeyQuery> _queryHandler;

    public GetApprenticeshipKey(IQueryHandler<GetApprenticeshipKeyResponse, GetApprenticeshipKeyQuery> queryHandler)
    {
        _queryHandler = queryHandler;
    }

    [Function(nameof(GetApprenticeshipKey))]
    public async Task<Guid?> Get([ActivityTrigger] Learner input)
    {
        var response = await _queryHandler.Get(new GetApprenticeshipKeyQuery(input.Ukprn, input.Uln));
        return response.ApprenticeshipKey;
    }
}