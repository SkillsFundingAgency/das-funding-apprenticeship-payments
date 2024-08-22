using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;

[ExcludeFromCodeCoverage]
/// <summary>
/// This class is used to get the current time from an external source and is only to be used for testing.
/// </summary>
/// <remarks>
/// Internally, this class uses a cache to reduce the number of calls to the external source.
/// </remarks>
public class IntegrationSystemClockService : ISystemClockService
{
    public DateTimeOffset UtcNow => GetUtcNow().Result.DateTime;

    public DateTime Now => GetUtcNow().Result.DateTime;

    private readonly HttpClient _httpClient;
    private readonly ILogger<IntegrationSystemClockService> _logger;

    // Cache fields
    private DateTimeOffset _cachedTime;
    private DateTimeOffset _lastFetchTime;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(5);

    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public IntegrationSystemClockService(HttpClient httpClient, ILogger<IntegrationSystemClockService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DateTimeOffset> GetUtcNow()
    {
        _logger.LogWarning("IntegrationSystemClockService is in use. This should not be used in production.");

        if (ShouldReturnCachedTime())
            return ReturnCachedTime();

        await _semaphore.WaitAsync();

        try
        {
            if (ShouldReturnCachedTime()) // Check again in case another thread has updated the cache
                return ReturnCachedTime();

            var response = await _httpClient.GetAsync(string.Empty);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to get system clock. Status code: {response.StatusCode}");
                return DateTime.UtcNow;
            }


            var json = await response.Content.ReadAsStringAsync();
            var systemClockResponse = JsonSerializer.Deserialize<SystemClockResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (systemClockResponse == null)
            {
                _logger.LogError("System clock response is invalid");
                return DateTime.UtcNow;
            }

            if (systemClockResponse.TimeValidUntil < DateTime.UtcNow)
            {
                _logger.LogInformation("System clock response is expired :{timeValidUntil}", systemClockResponse.TimeValidUntil);
                return DateTime.UtcNow;
            }

            _logger.LogInformation("Returning system clock time :{timeNow}", systemClockResponse.TimeNow);
            UpdateCache(systemClockResponse.TimeNow);
            return systemClockResponse.TimeNow;
        }
        finally
        {
            // Always release the semaphore
            _semaphore.Release();
        }
    }

    private bool ShouldReturnCachedTime()
    {
        return DateTimeOffset.UtcNow - _lastFetchTime < _cacheDuration;
    }

    private DateTimeOffset ReturnCachedTime()
    {
        _logger.LogInformation("Returning cached system clock time :{timeNow}", _cachedTime);
        return _cachedTime;
    }

    private void UpdateCache(DateTimeOffset time)
    {
        _cachedTime = time;
        _lastFetchTime = DateTimeOffset.UtcNow;
    }
}

[ExcludeFromCodeCoverage]
public class IntegrationSystemClockSettings
{
    public string Url { get; set; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public class SystemClockResponse
{
    /// <summary>
    /// This is the mocked current time the system should use
    /// </summary>
    public DateTime TimeNow { get; set; }

    /// <summary>
    /// The system should use the mocked time until this time
    /// </summary>
    public DateTime TimeValidUntil { get; set; }
}