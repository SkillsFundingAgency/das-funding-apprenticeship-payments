using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetApprenticeshipKey;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetApprenticeshipsWithDuePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetDuePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetLearnersInILR;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetProviders;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Query;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQueryServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IQueryHandler<GetApprenticeshipsWithDuePaymentsResponse, GetApprenticeshipsWithDuePaymentsQuery>, GetApprenticeshipsWithDuePaymentsQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetApprenticeshipKeyResponse, GetApprenticeshipKeyQuery>, GetApprenticeshipKeyQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetApprenticeshipsWithDuePaymentsResponse, GetApprenticeshipsWithDuePaymentsQuery>, GetApprenticeshipsWithDuePaymentsQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetDuePaymentsResponse, GetDuePaymentsQuery>, GetDuePaymentsQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetProvidersResponse, GetProvidersQuery>, GetProvidersQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<GetLearnersInILRQueryResponse, GetLearnersInILRQuery>, GetLearnersInILRQueryHandler>();
        return serviceCollection;
    }
}