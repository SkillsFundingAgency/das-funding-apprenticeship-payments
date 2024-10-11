namespace SFA.DAS.ApprenticeshipPayments.Query.GetApprenticeshipsWithDuePayments
{
    public interface IGetApprenticeshipsWithDuePaymentsQueryHandler
    {
        Task<GetApprenticeshipsWithDuePaymentsResponse> Get(GetApprenticeshipsWithDuePaymentsQuery command);
    }
}
