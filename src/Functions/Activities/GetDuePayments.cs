using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;
using SFA.DAS.Funding.ApprenticeshipPayments.Query;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetDuePayments;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;

public class GetDuePayments
{
    private readonly ILogger<GetDuePayments> _logger;
    private readonly IQueryHandler<GetDuePaymentsResponse, GetDuePaymentsQuery> _queryHandler;

    public GetDuePayments(
        ILogger<GetDuePayments> logger,
        IQueryHandler<GetDuePaymentsResponse, GetDuePaymentsQuery> queryHandler)
    {
        _logger = logger;
        _queryHandler = queryHandler;
    }

    [Function(nameof(GetDuePayments))]
    public async Task<IEnumerable<Guid>> Get([ActivityTrigger] GetDuePaymentsInput input)
    {
        using (_logger.BeginScope(input.GetLoggingScope()))
        {
            var payments = (await _queryHandler.Get(new GetDuePaymentsQuery(input.ApprenticeshipKey, input.CollectionDetails.CollectionYear, input.CollectionDetails.CollectionPeriod, InstalmentTypes.OnProgramme))).Payments;
            return payments.Select(x => x.Key);
        }

    }
}