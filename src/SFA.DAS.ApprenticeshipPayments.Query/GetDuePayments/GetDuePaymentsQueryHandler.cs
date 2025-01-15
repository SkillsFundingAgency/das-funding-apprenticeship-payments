using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetDuePayments;

public class GetDuePaymentsQueryHandler : IQueryHandler<GetDuePaymentsResponse, GetDuePaymentsQuery>
{
    private IApprenticeshipRepository _repository;

    public GetDuePaymentsQueryHandler(IApprenticeshipRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetDuePaymentsResponse> Get(GetDuePaymentsQuery query)
    {
        var apprenticeship = await _repository.Get(query.ApprenticeshipKey);
        var payments = apprenticeship.DuePayments(query.CollectionYear, query.CollectionPeriod);
        return new GetDuePaymentsResponse(payments.Select(x => new Payment(x.Key)));
    }
}