using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeshipPayments.Query.GetApprenticeshipsWithDuePayments;

namespace SFA.DAS.ApprenticeshipPayments.Query
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueryServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IGetApprenticeshipsWithDuePaymentsQueryHandler, GetApprenticeshipsWithDuePaymentsQueryHandler>();
            return serviceCollection;
        }
    }
}
