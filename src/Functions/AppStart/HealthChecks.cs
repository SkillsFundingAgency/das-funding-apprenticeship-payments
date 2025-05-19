using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.HealthChecks;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.AppStart;

public static class HealthChecks
{
    public static IServiceCollection AddFunctionHealthChecks(this IServiceCollection services, ApplicationSettings applicationSettings)
    {
        services.AddSingleton(sp => new FunctionHealthChecker(
            new DbHealthCheck(applicationSettings.DbConnectionString, sp.GetService<ILogger<DbHealthCheck>>()!),
            new ServiceBusReceiveHealthCheck(applicationSettings.NServiceBusConnectionString, "DasServiceBus", sp.GetService<ILogger<ServiceBusReceiveHealthCheck>>()!)
            ));


        return services;
    }
}
