using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Queries
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueryServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}
