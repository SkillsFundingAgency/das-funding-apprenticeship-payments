using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Interfaces;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Api.Requests;

public class GetAcademicYearsRequest : IGetApiRequest
{
    private readonly DateTime _searchDate;

    public GetAcademicYearsRequest(DateTime searchDate)
    {
        _searchDate = searchDate;
    }

    public string GetUrl => $"CollectionCalendar/academicYear/{_searchDate.ToString("yyyy-MM-dd HH:mm:ss")}";
}