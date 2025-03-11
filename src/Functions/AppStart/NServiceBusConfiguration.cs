using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Extensions;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.AppStart;

internal static class NServiceBusConfiguration
{
    internal static IHostBuilder ConfigureNServiceBusForSubscribe(this IHostBuilder hostBuilder, Action postProcessing = null)
    {

        hostBuilder.UseNServiceBus((config, endpointConfiguration) =>
        {
            endpointConfiguration.AdvancedConfiguration.EnableInstallers();

            endpointConfiguration.Transport.SubscriptionRuleNamingConvention = AzureRuleNameShortener.Shorten;
            endpointConfiguration.AdvancedConfiguration.SendFailedMessagesTo($"{Constants.EndpointName}-error");

            var serialization = endpointConfiguration.UseSerialization<SystemJsonSerializer>();
            serialization.Options(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter() },
                PropertyNameCaseInsensitive = true
            });
            
            endpointConfiguration.AdvancedConfiguration.Conventions().SetConventions();

            var value = config["ApplicationSettings:NServiceBusLicense"];
            if (!string.IsNullOrEmpty(value))
            {
                var decodedLicence = WebUtility.HtmlDecode(value);
                endpointConfiguration.AdvancedConfiguration.License(decodedLicence);
            }

            if(postProcessing != null)
            {
                postProcessing();
            }
        });

        return hostBuilder;
    }

    internal static class AzureRuleNameShortener
    {
        private const int AzureServiceBusRuleNameMaxLength = 50;

        public static string Shorten(Type type)
        {
            var ruleName = type.FullName;
            if (ruleName!.Length <= AzureServiceBusRuleNameMaxLength)
            {
                return ruleName;
            }

            var bytes = System.Text.Encoding.Default.GetBytes(ruleName);
            var hash = MD5.HashData(bytes);
            return new Guid(hash).ToString();
        }
    }
}
