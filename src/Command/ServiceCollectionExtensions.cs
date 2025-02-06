using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.FreezePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.UnfreezePayments;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ApplyFreezeAndUnfreeze;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ReleasePayment;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.SetLearnerReference;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;
using Azure.Identity;
using System.Security.Cryptography.X509Certificates;
using Azure.Security.KeyVault.Secrets;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton<IPaymentsGeneratedEventBuilder, PaymentsGeneratedEventBuilder>();
        serviceCollection.AddSingleton<IFinalisedOnProgammeLearningPaymentEventBuilder, FinalisedOnProgammeLearningPaymentEventBuilder>();
        serviceCollection.AddScoped<ICommandHandler<CalculateApprenticeshipPaymentsCommand>, CalculateApprenticeshipPaymentsCommandHandler>();
        serviceCollection.AddScoped<ICommandHandler<CalculateRequiredLevyAmountCommand>, CalculateRequiredLevyAmountCommandHandler>();
        serviceCollection.AddScoped<ICommandHandler<RecalculateApprenticeshipPaymentsCommand>, RecalculateApprenticeshipPaymentsCommandHandler>();
        serviceCollection.AddScoped<ICommandHandler<FreezePaymentsCommand>, FreezePaymentsCommandHandler>();
        serviceCollection.AddScoped<ICommandHandler<UnfreezePaymentsCommand>, UnfreezePaymentsCommandHandler>();
        serviceCollection.AddScoped<ICommandHandler<ApplyFreezeAndUnfreezeCommand>, ApplyFreezeAndUnfreezeCommandHandler>();
        serviceCollection.AddScoped<ICommandHandler<ReleasePaymentCommand>, ReleasePaymentCommandHandler>();
        serviceCollection.AddScoped<ICommandHandler<SetLearnerReferenceCommand>, SetLearnerReferenceCommandHandler>();
        serviceCollection.AddSystemClock(configuration);
        serviceCollection.AddHttpClient<IOuterApiClient, OuterApiClient>()
            .ConfigurePrimaryHttpMessageHandler(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<PaymentsOuterApi>>().Value;
                
                var credential = new DefaultAzureCredential();
                var secretClient = new SecretClient(new Uri(settings.CertificateKeyVault), credential);

                KeyVaultSecret secret = secretClient.GetSecret(settings.ApimCertificateName);
                var pfxBytes = Convert.FromBase64String(secret.Value);

                var certificate2 = new X509Certificate2(pfxBytes);
                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(certificate2);
                return handler;
            });
        return serviceCollection;
    }
}