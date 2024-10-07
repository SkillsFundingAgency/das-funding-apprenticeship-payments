namespace SFA.DAS.ApprenticeshipPayments.Query.GetApprenticeshipsWithDuePayments
{
    public class Apprenticeship
    {
        public Apprenticeship(Guid apprenticeshipKey)
        {
            ApprenticeshipKey = apprenticeshipKey;
        }

        public Guid ApprenticeshipKey { get; }
    }

    public class GetApprenticeshipsWithDuePaymentsResponse
    {
        public GetApprenticeshipsWithDuePaymentsResponse(List<Apprenticeship> apprenticeships)
        {
            Apprenticeships = apprenticeships;
        }

        public List<Apprenticeship> Apprenticeships { get; }
    }
}
