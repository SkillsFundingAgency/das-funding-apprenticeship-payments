using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Requests;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Responses;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetLearnersInILR;

public class GetLearnersInILRQueryHandler : IQueryHandler<GetLearnersInILRQueryResponse, GetLearnersInILRQuery>
{
    private readonly IOuterApiClient _outerApiClient;

    public GetLearnersInILRQueryHandler(IOuterApiClient outerApiClient)
    {
        _outerApiClient = outerApiClient;
    }

    public async Task<GetLearnersInILRQueryResponse> Get(GetLearnersInILRQuery query)
    {
        var response = await _outerApiClient.Get<List<LearnerReferenceResponse>>(new GetLearnersInILRRequest(query.Ukprn, query.AcademicYear));

        return new GetLearnersInILRQueryResponse(response.Body.Select(x => new Learner(x.Uln, x.LearnerRefNumber)));
    }
}