using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ApplyFreezeAndUnfreeze;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.FreezePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ReleasePayment;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.SetLearnerReference;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.UnfreezePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

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
                var handler = new HttpClientHandler();

                if (configuration.NotLocalOrAcceptanceTests())
                {
                    var credential = new DefaultAzureCredential();
                    var secretClient = new SecretClient(new Uri(settings.SecretClientUrl), credential);

                    KeyVaultSecret secret = secretClient.GetSecret(settings.SecretName);
                    var pfxBytes = Convert.FromBase64String(secret.Value);
                    var certificate2 = new X509Certificate2(pfxBytes);

                    handler.ClientCertificates.Add(certificate2);
                }

                return handler;
            });
        return serviceCollection;
    }
}