using AutoFixture;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Requests;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Responses;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;

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

        int currentYear = date.Year;
        int currentMonth = date.Month;

        int yearFrom;
        int yearTo;

        if (currentMonth > 7)
        {
            yearFrom = currentYear;
            yearTo = currentYear + 1;
        }
        else
        {
            yearFrom = currentYear - 1;
            yearTo = currentYear;
        }

        var json = new GetAcademicYearsResponse
        {
            AcademicYear = GetAcademicYearString(yearFrom, yearTo),
            StartDate = new DateTime(yearFrom, 8, 1),
            EndDate = new DateTime(yearTo, 7, 31),
            HardCloseDate = new DateTime(yearTo, 10, 15, 23, 59, 59)
        };

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

    private static string GetAcademicYearString(int yearFrom, int yearTo)
    {
        int from = yearFrom - 2000; // removing 2000 turns 2023 into 23. This should work until the year 2100 at which point a refactor is needed :)
        int to = yearTo - 2000;

        return $"{from}{to}";
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
