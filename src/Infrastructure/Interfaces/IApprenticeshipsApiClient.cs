using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;

public interface IApprenticeshipsApiClient
{
    Task<ApiResponse<TResponse>> Get<TResponse>(IGetApiRequest request);
}
