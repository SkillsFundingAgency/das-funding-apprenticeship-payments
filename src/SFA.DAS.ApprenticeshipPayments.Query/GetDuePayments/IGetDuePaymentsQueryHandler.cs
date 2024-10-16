namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetDuePayments
{
    public interface IGetDuePaymentsQueryHandler
    {
        Task<GetDuePaymentsResponse> Get(GetDuePaymentsQuery command);
    }
}
