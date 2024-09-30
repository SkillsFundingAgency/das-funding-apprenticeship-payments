using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSystemClock(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var useIntegrationSystemClock = configuration.GetValue<bool>("IntegrationSystemClockSettings:UseIntegrationSystemClock");

        if(useIntegrationSystemClock)
        {
            serviceCollection.Configure<IntegrationSystemClockSettings>(configuration.GetSection(nameof(IntegrationSystemClockSettings)));
            serviceCollection.AddSingleton(cfg => cfg.GetService<IOptions<IntegrationSystemClockSettings>>()!.Value);

            serviceCollection.AddSingleton<ISystemClockService>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();
                var settings = provider.GetRequiredService<IOptions<IntegrationSystemClockSettings>>().Value;
                httpClient.BaseAddress = new Uri(settings.Url);
                var logger = provider.GetRequiredService<ILogger<IntegrationSystemClockService>>();
                return new IntegrationSystemClockService(httpClient, logger);
            });

        }
        else
        {
            serviceCollection.AddSingleton<ISystemClockService, SystemClockService>();
        }

        return serviceCollection;
    }
}
