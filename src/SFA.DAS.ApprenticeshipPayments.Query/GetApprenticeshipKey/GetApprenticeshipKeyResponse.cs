namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetApprenticeshipKey;

public class GetApprenticeshipKeyResponse
{
    public GetApprenticeshipKeyResponse(Guid? apprenticeshipKey)
    {
        ApprenticeshipKey = apprenticeshipKey;
    }

    public Guid? ApprenticeshipKey { get; }
}