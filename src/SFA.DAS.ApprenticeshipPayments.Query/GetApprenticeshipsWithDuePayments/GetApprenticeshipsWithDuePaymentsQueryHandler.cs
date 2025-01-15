using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetApprenticeshipsWithDuePayments;

public class GetApprenticeshipsWithDuePaymentsQueryHandler : IQueryHandler<GetApprenticeshipsWithDuePaymentsResponse, GetApprenticeshipsWithDuePaymentsQuery>
{
    private IApprenticeshipQueryRepository _repository;

    public GetApprenticeshipsWithDuePaymentsQueryHandler(IApprenticeshipQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetApprenticeshipsWithDuePaymentsResponse> Get(GetApprenticeshipsWithDuePaymentsQuery query)
    {
        var apprenticeships = await _repository.GetWithDuePayments(query.CollectionYear, query.CollectionPeriod);
        return new GetApprenticeshipsWithDuePaymentsResponse(apprenticeships.Select(x => new Apprenticeship(x)));
    }
}