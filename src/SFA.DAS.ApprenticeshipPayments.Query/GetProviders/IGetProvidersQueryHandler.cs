namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetProviders
{
    public interface IGetProvidersQueryHandler
    {
        Task<GetProvidersResponse> Get(GetProvidersQuery command);
    }
}
