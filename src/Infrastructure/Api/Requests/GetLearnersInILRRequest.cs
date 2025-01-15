using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Requests;

public class GetLearnersInILRRequest : IGetApiRequest
{
    private readonly long _ukprn;
    private readonly short _academicYear;

    public GetLearnersInILRRequest(long ukprn, short academicYear)
    {
        _ukprn = ukprn;
        _academicYear = academicYear;
    }

    public string GetUrl => $"ILR/{_ukprn}/{_academicYear}";
}