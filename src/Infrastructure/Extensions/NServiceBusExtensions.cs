using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Extensions;

public static class NServiceBusExtensions
{
    public static void ConfigureNServiceBusForSend<T>(this IServiceCollection services, string fullyQualifiedNamespace, Func<IEndpointInstance, T> endpointDiWrapper) where T : class
    {
        var endpointConfiguration = new EndpointConfiguration("SFA.DAS.Funding.ApprenticeshipPayments");
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.SendOnly();

        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        transport.CustomTokenCredential(fullyQualifiedNamespace, new DefaultAzureCredential());
        endpointConfiguration.Conventions().SetConventions();

        var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        services.AddSingleton(endpointDiWrapper(endpointInstance));
    }

    public static void SetConventions(this ConventionsBuilder conventions)
    {
        conventions.DefiningEventsAs(IsEvent);
        conventions.DefiningCommandsAs(IsCommand);
        conventions.DefiningMessagesAs(t => false);
    }

    public static string GetFullyQualifiedNamespace(this string serviceBusConnectionString)
    {
        if (string.IsNullOrWhiteSpace(serviceBusConnectionString))
        {
            throw new ArgumentException("Service Bus connection string cannot be null or empty.", nameof(serviceBusConnectionString));
        }

        var parts = serviceBusConnectionString.Split(';');
        foreach (var part in parts)
        {
            if (part.StartsWith("Endpoint=", StringComparison.OrdinalIgnoreCase))
            {
                var endpoint = part.Split('=')[1]; // Extract after "Endpoint="
                return new Uri(endpoint).Host; // Extract only the hostname
            }
        }

        throw new FormatException("Invalid Service Bus connection string: Fully Qualified Namespace not found.");
    }

    private static bool IsEvent(Type t) => t.Name.EndsWith("Event");

    private static bool IsCommand(Type t)
    {
        return (t.Namespace != null && t.Namespace.StartsWith("SFA.DAS.Payments.FundingSource.Messages.Commands", StringComparison.CurrentCultureIgnoreCase));
    }
}
