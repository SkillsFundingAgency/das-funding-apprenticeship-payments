using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;

public interface IOuterApiClient
{
    Task<ApiResponse<TResponse>> Get<TResponse>(IGetApiRequest request);
}
