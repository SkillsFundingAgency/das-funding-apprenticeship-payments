namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetApprenticeshipsWithDuePayments;

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
    public GetApprenticeshipsWithDuePaymentsResponse(IEnumerable<Apprenticeship> apprenticeships)
    {
        Apprenticeships = apprenticeships;
    }

    public IEnumerable<Apprenticeship> Apprenticeships { get; }
}