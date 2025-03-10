using Azure.Identity;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Extensions;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.AppStart;

internal static class NServiceBusConfiguration
{
    internal static IHostBuilder ConfigureNServiceBusForSubscribe(this IHostBuilder hostBuilder, Action postProcessing = null)
    {

        hostBuilder.UseNServiceBus((config, endpointConfiguration) =>
        {
            endpointConfiguration.LogDiagnostics();
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

            //CheckCreateQueues(config);

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

    /// <summary>
    /// Check if the queues exist and create them if they don't
    /// </summary>
    private static void CheckCreateQueues(IConfiguration configuration)
    {
        var queueTriggers = GetQueueTriggers();

        var connectionString = configuration["ApplicationSettings:NServiceBusConnectionString"];
        var fullyQualifiedNamespace = connectionString.GetFullyQualifiedNamespace();
        var adminClient = new ServiceBusAdministrationClient(fullyQualifiedNamespace, new DefaultAzureCredential());

        foreach (var queueTrigger in queueTriggers)
        {
            var errorQueue = $"{queueTrigger.QueueName}-error";

            // Create the error queue if it doesn't exist
            if (!adminClient.QueueExistsAsync(errorQueue).GetAwaiter().GetResult())
            {
                adminClient.CreateQueueAsync(errorQueue).GetAwaiter().GetResult();
            }

            if (!adminClient.QueueExistsAsync(queueTrigger.QueueName).GetAwaiter().GetResult())
            {
                // Create the queue
                var queue = new CreateQueueOptions(queueTrigger.QueueName)
                {
                    ForwardDeadLetteredMessagesTo = errorQueue,
                    MaxDeliveryCount = 5
                };

                adminClient.CreateQueueAsync(queue).GetAwaiter().GetResult();
            }
            else
            {
                // Update the queue if it doesn't have the error queue set
                var queueProperties = adminClient.GetQueueAsync(queueTrigger.QueueName).GetAwaiter().GetResult();

                if (string.IsNullOrEmpty(queueProperties.Value.ForwardDeadLetteredMessagesTo))
                {
                    queueProperties.Value.ForwardDeadLetteredMessagesTo = errorQueue;
                    queueProperties.Value.MaxDeliveryCount = 5;

                    adminClient.UpdateQueueAsync(queueProperties.Value).GetAwaiter().GetResult();
                }
            }

            CreateSubscription(adminClient, "bundle-1", queueTrigger).GetAwaiter().GetResult();
        }
    }

    private static async Task CreateSubscription(ServiceBusAdministrationClient adminClient, string topicName, QueueTrigger queueTrigger)
    {
        if (await adminClient.SubscriptionExistsAsync(topicName, queueTrigger.QueueName)) return;


        var createSubscriptionOptions = new CreateSubscriptionOptions(topicName, queueTrigger.QueueName)
        {
            ForwardTo = queueTrigger.QueueName,
            UserMetadata = $"Subscribed to {queueTrigger.QueueName}"
        };
        var createRuleOptions = new CreateRuleOptions()
        {
            Filter = new FalseRuleFilter()
        };

        await adminClient.CreateSubscriptionAsync(createSubscriptionOptions, createRuleOptions);
        await CreateNewSqlFilter(adminClient, queueTrigger.QueueName, topicName, new[] { queueTrigger.EventName });
    }

    private static async Task CreateNewSqlFilter(ServiceBusAdministrationClient adminClient, string subscriptionName, string topicName, IEnumerable<string> filterEventTypes)
    {

        var sqlExpression = "[NServiceBus.EnclosedMessageTypes] LIKE '%" +
                            string.Join("%' OR [NServiceBus.EnclosedMessageTypes] LIKE '%",
                                filterEventTypes) + "%'";

        await adminClient.CreateRuleAsync(topicName, subscriptionName, new CreateRuleOptions
        {
            Name = $"FilterRule{Guid.NewGuid()}",
            Filter = new SqlRuleFilter(sqlExpression)
        });

    }

    private static IEnumerable<QueueTrigger> GetQueueTriggers()
    {
        var allAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => x.GetName().FullName.Contains("SFA.DAS.Funding"));

        var queueTriggers = allAssemblies
            .SelectMany(assembly => assembly.GetTypes())
            .SelectMany(type => type.GetMethods())
            .SelectMany(method => method.GetParameters())
            .Where(parameter => parameter.GetCustomAttributes(typeof(ServiceBusTriggerAttribute), false).Any())
            .Select(parameter =>
            {
                var attribute = (ServiceBusTriggerAttribute)parameter.GetCustomAttributes(typeof(ServiceBusTriggerAttribute), false).FirstOrDefault();
                return new QueueTrigger { QueueName = attribute.QueueName, EventName = parameter.ParameterType.FullName };
            });


        return queueTriggers;
    }
}

public class QueueTrigger
{
    public string QueueName { get; set; }
    public string EventName { get; set; }
}