using Microsoft.Extensions.Logging;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Requests;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Responses;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetLearnersInILR;

public class GetLearnersInILRQueryHandler : IQueryHandler<GetLearnersInILRQueryResponse, GetLearnersInILRQuery>
{
    private readonly IOuterApiClient _outerApiClient;
    private readonly ILogger<GetLearnersInILRQueryHandler> _logger;

    public GetLearnersInILRQueryHandler(IOuterApiClient outerApiClient, ILogger<GetLearnersInILRQueryHandler> logger)
    {
        _outerApiClient = outerApiClient;
        _logger = logger;
    }

    public async Task<GetLearnersInILRQueryResponse> Get(GetLearnersInILRQuery query)
    {
        var response = await _outerApiClient.Get<List<LearnerReferenceResponse>>(new GetLearnersInILRRequest(query.Ukprn, query.AcademicYear));

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Error calling learners API - {response.ErrorContent}");
            throw new Exception($"Error calling learners API - {response.ErrorContent}");
        }

        return new GetLearnersInILRQueryResponse(response.Body.Select(x => new Learner(x.Uln, x.LearnerRefNumber)));
    }
}