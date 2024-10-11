namespace SFA.DAS.Funding.ApprenticeshipPayments.DataAccess;

public interface ISqlAzureIdentityTokenProvider
{
    Task<string> GetAccessTokenAsync();
    string GetAccessToken();
}