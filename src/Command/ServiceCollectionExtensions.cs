using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPaymentsGeneratedEventBuilder, PaymentsGeneratedEventBuilder>();
            serviceCollection.AddScoped<ICalculateApprenticeshipPaymentsCommandHandler, CalculateApprenticeshipPaymentsCommandHandler>();
            return serviceCollection;
        }
    }
}
