using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.HealthChecks;

// Because the lifecycle of Azure Functions will generate singletons multiple times
// we need to store a static reference to the health check classes
// this will allow the health checks to cache their results
public class FunctionHealthChecker
{
    private static DbHealthCheck? _dbHealthCheck;
    private static ServiceBusReceiveHealthCheck? _dasServiceBusHealthCheck;

    internal FunctionHealthChecker(
        DbHealthCheck dbHealthCheck, 
        ServiceBusReceiveHealthCheck dasServiceBusHealthCheck)
    {
        if (_dbHealthCheck == null && _dasServiceBusHealthCheck == null)
        {
            _dbHealthCheck = dbHealthCheck;
            _dasServiceBusHealthCheck = dasServiceBusHealthCheck;
        }
    }

    internal async Task<bool> HealthCheck(CancellationToken cancellationToken)
    {
        var healthCheckContext = new HealthCheckContext();

        var dbResult = await _dbHealthCheck.CheckHealthAsync(healthCheckContext, cancellationToken);
        if (dbResult.Status != HealthStatus.Healthy)
        {
            return false;
        }

        var dasServiceBusResult = await _dasServiceBusHealthCheck.CheckHealthAsync(healthCheckContext, cancellationToken);
        if (dasServiceBusResult.Status != HealthStatus.Healthy)
        {
            return false;
        }

        return true;
    }
}
