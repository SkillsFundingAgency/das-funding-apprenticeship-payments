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

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IPaymentsGeneratedEventBuilder, PaymentsGeneratedEventBuilder>();
            serviceCollection.AddSingleton<IFinalisedOnProgammeLearningPaymentEventBuilder, FinalisedOnProgammeLearningPaymentEventBuilder>();
            serviceCollection.AddScoped<ICalculateApprenticeshipPaymentsCommandHandler, CalculateApprenticeshipPaymentsCommandHandler>();
            serviceCollection.AddScoped<ICalculateRequiredLevyAmountCommandHandler, CalculateRequiredLevyAmountCommandHandler>();
            serviceCollection.AddScoped<IRecalculateApprenticeshipPaymentsCommandHandler, RecalculateApprenticeshipPaymentsCommandHandler>();
            serviceCollection.AddScoped<IFreezePaymentsCommandHandler, FreezePaymentsCommandHandler>();
            serviceCollection.AddScoped<IUnfreezePaymentsCommandHandler, UnfreezePaymentsCommandHandler>();
            serviceCollection.AddScoped<IApplyFreezeAndUnfreezeCommandHandler, ApplyFreezeAndUnfreezeCommandHandler>();
            serviceCollection.AddScoped<IReleasePaymentCommandHandler, ReleasePaymentCommandHandler>();
            serviceCollection.AddScoped<ISetLearnerReferenceCommandHandler, SetLearnerReferenceCommandHandler>();
            serviceCollection.AddSystemClock(configuration);
            serviceCollection.AddHttpClient<IApprenticeshipsApiClient, ApprenticeshipsApiClient>();
            return serviceCollection;
        }
    }
}
