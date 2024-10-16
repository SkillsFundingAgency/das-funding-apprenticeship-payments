namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetApprenticeshipKey
{
    public interface IGetApprenticeshipKeyQueryHandler
    {
        Task<GetApprenticeshipKeyResponse> Get(GetApprenticeshipKeyQuery command);
    }
}
