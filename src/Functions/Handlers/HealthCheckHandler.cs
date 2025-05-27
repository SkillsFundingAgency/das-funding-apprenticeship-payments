using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.HealthChecks;
using System.Net.Http;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Handlers;

public class HealthCheckHandler(FunctionHealthChecker functionHealthChecker)
{
    [Function(nameof(HealthCheck))]
    public async Task<IActionResult> HealthCheck([HttpTrigger(AuthorizationLevel.Function, "get", Route = "HealthCheck")] HttpRequestMessage req, CancellationToken cancellationToken)
    {
        var result = await functionHealthChecker.HealthCheck(cancellationToken);
        if (!result)
        {
            return new ObjectResult("Unhealthy") { StatusCode = 503 };
        }

        return new OkObjectResult("Healthy");
    }
}
