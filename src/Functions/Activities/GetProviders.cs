using System.Collections.Generic;
using SFA.DAS.Funding.ApprenticeshipPayments.Query;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetProviders;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;

public class GetProviders
{
    private readonly IQueryHandler<GetProvidersResponse, GetProvidersQuery> _queryHandler;

    public GetProviders(IQueryHandler<GetProvidersResponse, GetProvidersQuery> queryHandler)
    {
        _queryHandler = queryHandler;
    }

    [FunctionName(nameof(GetProviders))]
    public async Task<IEnumerable<long>> Get([ActivityTrigger] object input)
    {
        var providers = (await _queryHandler.Get(new GetProvidersQuery())).Providers;
        return providers.Select(x => x.Ukprn);
    }
}