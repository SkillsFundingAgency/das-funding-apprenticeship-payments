using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}
