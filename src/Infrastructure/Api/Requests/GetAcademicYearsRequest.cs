using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Requests;

public class GetAcademicYearsRequest : IGetApiRequest
{
    private readonly DateTime _searchDate;

    public GetAcademicYearsRequest(DateTime searchDate)
    {
        _searchDate = searchDate;
    }

    public string GetUrl => $"CollectionCalendar/academicYear/{_searchDate.ToString("yyyy-MM-dd HH:mm:ss")}";
}