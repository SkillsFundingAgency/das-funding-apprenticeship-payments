using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;

public interface IApiClient
{
    Task<ApiResponse<TResponse>> Get<TResponse>(IGetApiRequest request);
}
