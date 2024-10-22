using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeshipPayments.Query.GetApprenticeshipsWithDuePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetApprenticeshipKey;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetDuePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetLearnersInILR;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetProviders;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Query
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueryServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IGetApprenticeshipsWithDuePaymentsQueryHandler, GetApprenticeshipsWithDuePaymentsQueryHandler>();
            serviceCollection.AddScoped<IGetApprenticeshipKeyQueryHandler, GetApprenticeshipKeyQueryHandler>();
            serviceCollection.AddScoped<IGetApprenticeshipsWithDuePaymentsQueryHandler, GetApprenticeshipsWithDuePaymentsQueryHandler>();
            serviceCollection.AddScoped<IGetDuePaymentsQueryHandler, GetDuePaymentsQueryHandler>();
            serviceCollection.AddScoped<IGetProvidersQueryHandler, GetProvidersQueryHandler>();
            serviceCollection.AddScoped<IGetLearnersInILRQueryHandler, GetLearnersInILRQueryHandler>();
            return serviceCollection;
        }
    }
}
