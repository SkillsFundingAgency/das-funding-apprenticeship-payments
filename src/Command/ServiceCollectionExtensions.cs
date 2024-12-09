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
        serviceCollection.AddHttpClient<IApprenticeshipsApiClient, ApprenticeshipsApiClient>();
        serviceCollection.AddHttpClient<IOuterApiClient, OuterApiClient>();
        return serviceCollection;
    }
}