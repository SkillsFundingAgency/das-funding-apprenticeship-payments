using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPaymentsGeneratedEventBuilder, PaymentsGeneratedEventBuilder>();
            serviceCollection.AddSingleton<IFinalisedOnProgammeLearningPaymentEventBuilder, FinalisedOnProgammeLearningPaymentEventBuilder>();
            serviceCollection.AddScoped<ICalculateApprenticeshipPaymentsCommandHandler, CalculateApprenticeshipPaymentsCommandHandler>();
            serviceCollection.AddScoped<IProcessUnfundedPaymentsCommandHandler, ProcessUnfundedPaymentsCommandHandler>();
            serviceCollection.AddScoped<ICalculateRequiredLevyAmountCommandHandler, CalculateRequiredLevyAmountCommandHandler>();
            return serviceCollection;
        }
    }
}
