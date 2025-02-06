namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;

#pragma warning disable CS8618
public class PaymentsOuterApi
{
    public string Key { get; set; }
    public string BaseUrl { get; set; }
    public string CertificateKeyVault { get; set; }
    public string ApimCertificateName { get; set; }
}
#pragma warning restore CS8618