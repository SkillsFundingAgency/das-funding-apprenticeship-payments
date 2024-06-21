using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

public class PerformanceMonitor : IDisposable
{
    public string MeasureAction { get; private set; }
    public Stopwatch Stopwatch { get; private set; }
    private ILogger _logger;

    public PerformanceMonitor(ILogger logger, string measureAction)
    {
        _logger = logger;
        MeasureAction = measureAction;
        Stopwatch = new Stopwatch();
        Stopwatch.Start();
    }

    public void Dispose()
    {
        Stopwatch.Stop();
        _logger.LogInformation("PerformanceMonitor: {measureAction} took {elapsedMilliseconds}ms", MeasureAction, Stopwatch.ElapsedMilliseconds);
    }
}

public class PerformanceMonitor<T> : PerformanceMonitor
{
    public PerformanceMonitor(ILogger<T> logger, string measureAction) : base(logger, measureAction)
    {
    }
}

public static class PerformanceMonitorExtensions
{
    public static PerformanceMonitor<T> LogPerformance<T>(this ILogger<T> logger, string measureAction)
    {
        return new PerformanceMonitor<T>(logger, measureAction);
    }

    public static PerformanceMonitor LogPerformance(this ILogger logger, string measureAction)
    {
        return new PerformanceMonitor(logger, measureAction);
    }
}
