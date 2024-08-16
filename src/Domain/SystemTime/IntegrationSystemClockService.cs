using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;

[ExcludeFromCodeCoverage]
public class IntegrationSystemClockService : ISystemClockService
{
    public DateTimeOffset UtcNow => GetUtcNow().Result.DateTime;

    public DateTime Now => GetUtcNow().Result.DateTime;

    private readonly HttpClient _httpClient;
    private readonly ILogger<IntegrationSystemClockService> _logger;

    public IntegrationSystemClockService(HttpClient httpClient, IOptions<IntegrationSystemClockSettings> config, ILogger<IntegrationSystemClockService> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(config.Value.Url);
        _logger = logger;
    }

    public async Task<DateTimeOffset> GetUtcNow()
    {
        _logger.LogWarning("IntegrationSystemClockService is in use. This should not be used in production.");

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
        return systemClockResponse.TimeNow;
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