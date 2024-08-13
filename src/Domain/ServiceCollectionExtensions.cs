using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Api;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Factories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Interfaces;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection serviceCollection)
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
                .AddSingleton<ISystemClockService, SystemClockService>();

            serviceCollection.AddHttpClient<IApiClient, ApiClient>(); 

            return serviceCollection;
        }
    }
}
