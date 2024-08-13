using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Api;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Interfaces;

public interface IApiClient
{
    Task<ApiResponse<TResponse>> Get<TResponse>(IGetApiRequest request);
}
