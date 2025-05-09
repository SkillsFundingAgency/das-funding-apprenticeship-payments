using AutoFixture;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Requests;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Responses;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests;

internal class TestOuterApi : IOuterApiClient
{
    private readonly TestContext _context;
    
    public TestOuterApi(TestContext context)
    {
        _context = context;
    }

    public Task<ApiResponse<TResponse>> Get<TResponse>(IGetApiRequest request)
    {
        if (request is GetLearnersInILRRequest getLearnersInIlrRequest)
        {
            return GetLearnersInILR<TResponse>(getLearnersInIlrRequest);
        }

        if (request is GetAcademicYearsRequest academicYearsRequest)
        {
            return GetAcademicYear<TResponse>(academicYearsRequest);
        }

        throw new NotImplementedException();
    }

    public Task<ApiResponse<TResponse>> GetAcademicYear<TResponse>(GetAcademicYearsRequest request)
    {
        var date = GetSearchDate(request);

        var json = AcademicYearHelper.GetMockedAcademicYear<GetAcademicYearsResponse>(date);

        // Check if the TResponse is assignable from the GetAcademicYearsResponse type
        if (typeof(TResponse).IsAssignableFrom(typeof(GetAcademicYearsResponse)))
        {
            return Task.FromResult(new ApiResponse<TResponse>((TResponse)(object)json, System.Net.HttpStatusCode.OK, string.Empty));
        }
        else
        {
            throw new InvalidCastException($"Cannot cast {typeof(GetAcademicYearsResponse)} to {typeof(TResponse)}");
        }
    }
    private static DateTime GetSearchDate(GetAcademicYearsRequest request)
    {
        var dateString = request.GetUrl.Split("/").Last();
        return DateTime.Parse(dateString);
    }

    public Task<ApiResponse<TResponse>> GetLearnersInILR<TResponse>(GetLearnersInILRRequest request)
    {
        var json = new List<LearnerReferenceResponse>
        (
            _context.Ulns.Select(x => new LearnerReferenceResponse { Uln = x, LearnerRefNumber = _context.Fixture.Create<string>() }).ToList()
        );

        // Check if the TResponse is assignable from the GetLearnersInILRResponse type
        if (typeof(TResponse).IsAssignableFrom(typeof(List<LearnerReferenceResponse>)))
        {
            return Task.FromResult(new ApiResponse<TResponse>((TResponse)(object)json, System.Net.HttpStatusCode.OK, string.Empty));
        }
        else
        {
            throw new InvalidCastException($"Cannot cast {typeof(List<LearnerReferenceResponse>)} to {typeof(TResponse)}");
        }
    }
}
