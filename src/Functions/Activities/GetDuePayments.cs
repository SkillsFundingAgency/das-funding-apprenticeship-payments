using System.Collections.Generic;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;
using SFA.DAS.Funding.ApprenticeshipPayments.Query;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetDuePayments;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;

public class GetDuePayments
{
    private readonly IQueryHandler<GetDuePaymentsResponse, GetDuePaymentsQuery> _queryHandler;

    public GetDuePayments(IQueryHandler<GetDuePaymentsResponse, GetDuePaymentsQuery> queryHandler)
    {
        _queryHandler = queryHandler;
    }

    [FunctionName(nameof(GetDuePayments))]
    public async Task<IEnumerable<Guid>> Get([ActivityTrigger] GetDuePaymentsInput input)
    {
        var payments = (await _queryHandler.Get(new GetDuePaymentsQuery(input.ApprenticeshipKey, input.CollectionDetails.CollectionYear, input.CollectionDetails.CollectionPeriod))).Payments;
        return payments.Select(x => x.Key);
    }
}