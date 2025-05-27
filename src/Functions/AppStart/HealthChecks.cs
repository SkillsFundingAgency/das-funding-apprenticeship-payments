using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.HealthChecks;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.AppStart;

public static class HealthChecks
{
    public static IServiceCollection AddFunctionHealthChecks(this IServiceCollection services, ApplicationSettings applicationSettings, bool sqlConnectionNeedsAccessToken)
    {
        services.AddSingleton(sp => new FunctionHealthChecker(
            new DbHealthCheck(applicationSettings.DbConnectionString, sp.GetService<ILogger<DbHealthCheck>>()!, sp.GetSqlAzureIdentityTokenProvider(sqlConnectionNeedsAccessToken)),
            new ServiceBusReceiveHealthCheck(applicationSettings.NServiceBusConnectionString, "DasServiceBus", sp.GetService<ILogger<ServiceBusReceiveHealthCheck>>()!)
            ));


        return services;
    }
}

public static class ServiceProviderExtensions
{
    /// <summary>
    /// Only returns token provider if required, else returns null
    /// </summary>
    public static ISqlAzureIdentityTokenProvider? GetSqlAzureIdentityTokenProvider(this IServiceProvider services, bool sqlConnectionNeedsAccessToken)
    {
        if (!sqlConnectionNeedsAccessToken)
            return null;

        return services.GetService<ISqlAzureIdentityTokenProvider>();
    }
}