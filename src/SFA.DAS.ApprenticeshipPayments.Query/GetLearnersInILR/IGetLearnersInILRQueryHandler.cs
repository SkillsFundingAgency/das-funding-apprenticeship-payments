namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetLearnersInILR
{
    public interface IGetLearnersInILRQueryHandler
    {
        Task<GetLearnersInILRQueryResponse> Get(GetLearnersInILRQuery command);
    }
}
