﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Factories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.Scan(scan =>
                {
                    scan.FromAssembliesOf(typeof(ServiceCollectionExtensions))
                        .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
                        .AsImplementedInterfaces()
                        .WithTransientLifetime();
                })
                .AddScoped<IDomainEventDispatcher, DomainEventDispatcher>()
                .AddScoped<IApprenticeshipFactory, ApprenticeshipFactory>();

            return serviceCollection;
        }
    }
}
