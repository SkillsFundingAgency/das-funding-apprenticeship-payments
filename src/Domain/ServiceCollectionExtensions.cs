using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Api;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Factories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Interfaces;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Scan(scan =>
                {
                    scan.FromAssembliesOf(typeof(ServiceCollectionExtensions))
                        .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
                        .AsImplementedInterfaces()
                        .WithTransientLifetime();
                })
                .AddScoped<IDomainEventDispatcher, DomainEventDispatcher>()
                .AddScoped<IApprenticeshipFactory, ApprenticeshipFactory>()
                .AddSystemClock(configuration);

            serviceCollection.AddHttpClient<IApiClient, ApiClient>(); 

            return serviceCollection;
        }
    }
}
