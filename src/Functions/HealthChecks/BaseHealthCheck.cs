using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.HealthChecks;

[ExcludeFromCodeCoverage]
// Note: This class does not currently implement IHealthCheck because azure functions do not support it. But it does return
// a HealthCheckResult. This is for 2 reasons:
// 1. It keeps the implementation consistent with function earnings repository.
// 2. If a inner api is added in the future (like earnings repo), it will be easier to implement IHealthCheck.
internal abstract class BaseHealthCheck<T>
{
    private HealthCheckResult _cachedResult;
    private DateTimeOffset _lastCheckTime = DateTimeOffset.MinValue;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(30);
    private readonly object _lock = new();
    private readonly ILogger<T> _logger;

    protected BaseHealthCheck(ILogger<T> logger)
    {
        _logger = logger;
    }

    internal async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var cachedResult = GetCachedResult();
        if (cachedResult != null)
        {
            return cachedResult.Value;
        }

        var result = await HealthCheck(cancellationToken);

        SetCachedResult(result);
        return result;
    }

    protected abstract Task<HealthCheckResult> HealthCheck(CancellationToken cancellationToken);

    protected void LogError(string error, Exception ex)
    {
        _logger.LogError($"{error} Exception:{ex.Message}", ex);
    }

    private HealthCheckResult? GetCachedResult()
    {
        lock (_lock)
        {
            if (DateTimeOffset.UtcNow - _lastCheckTime < _cacheDuration)
            {
                return _cachedResult;
            }
        }

        return null;
    }

    private void SetCachedResult(HealthCheckResult result)
    {
        lock (_lock)
        {
            _cachedResult = result;
            _lastCheckTime = DateTimeOffset.UtcNow;
        }
    }

}
